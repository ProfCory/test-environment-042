using System;
using System.Collections.Generic;
using System.Linq;
using AnotherLife.Core.Data;

namespace AnotherLife.Core.Rules
{
    public enum PayMethod { Cash, Card }

    public static class Inventory
    {
        public const string BankCard = "bank_card";
        public const string FakeId = "id_fake";

        public static int Count (PlayerState p, string item) =>
            p.Inventory.Where(s => s.Item == item).Sum(s => s.Count);

        public static bool Has (PlayerState p, string item) => Count(p, item) > 0;

        public static void Add (PlayerState p, string item, int count = 1, string meta = null)
        {
            var stack = p.Inventory.FirstOrDefault(s => s.Item == item && s.Meta == meta);
            if (stack != null) stack.Count += count;
            else p.Inventory.Add(new ItemStack { Item = item, Count = count, Meta = meta });
        }

        public static bool Remove (PlayerState p, string item, int count = 1)
        {
            if (Count(p, item) < count) return false;
            foreach (var s in p.Inventory.Where(s => s.Item == item).ToList())
            {
                int take = Math.Min(s.Count, count);
                s.Count -= take;
                count -= take;
                if (s.Count == 0) p.Inventory.Remove(s);
                if (count == 0) break;
            }
            return true;
        }

        // Removes and returns every stack that `keep` doesn't protect.
        public static List<ItemStack> Strip (PlayerState p, Func<ItemDef, bool> keep, GameData data)
        {
            var lost = p.Inventory.Where(s => { var def = data.Item(s.Item); return def == null || !keep(def); }).ToList();
            foreach (var s in lost) p.Inventory.Remove(s);
            return lost;
        }
    }

    public static class Economy
    {
        public static bool CanUseCard (PlayerState p) => p.HasBankAccount && Inventory.Has(p, Inventory.BankCard);

        public static bool CanAfford (PlayerState p, long amount, PayMethod method, EconomyRules rules) =>
            method == PayMethod.Cash
                ? p.Cash >= amount
                : CanUseCard(p) && p.Bank - amount >= rules.BankOverdraftLimit;

        public static bool TrySpend (PlayerState p, long amount, PayMethod method, EconomyRules rules)
        {
            if (amount < 0 || !CanAfford(p, amount, method, rules)) return false;
            if (method == PayMethod.Cash) p.Cash -= amount;
            else p.Bank -= amount;
            UpdateCollectors(p, rules);
            return true;
        }

        // Bills the bank must take regardless of the overdraft limit (hospital, fines).
        public static void ChargeBankForced (PlayerState p, long amount, EconomyRules rules)
        {
            p.Bank -= amount;
            UpdateCollectors(p, rules);
        }

        public static void Deposit (PlayerState p, long amount, EconomyRules rules)
        {
            p.Bank += amount;
            UpdateCollectors(p, rules);
        }

        // Leg breakers start at the overdraft limit and keep coming until the balance recovers.
        public static void UpdateCollectors (PlayerState p, EconomyRules rules)
        {
            if (!p.HasBankAccount) { p.CollectorsActive = false; return; }
            if (p.Bank <= rules.BankOverdraftLimit) p.CollectorsActive = true;
            else if (p.CollectorsActive && p.Bank >= rules.CollectorsStopAtBalance) p.CollectorsActive = false;
        }

        // Bail-bond thugs take whatever cash you carry until the bond is paid.
        public static long ThugShakedown (PlayerState p)
        {
            long take = Math.Min(p.Cash, p.BondDebt);
            p.Cash -= take;
            p.BondDebt -= take;
            return take;
        }

        // Decimal so a 10% markup on $100 is $110, not $110.00000000000001 rounded up to $111.
        public static long OrderPrice (long shopPrice, EconomyRules rules) =>
            (long)Math.Ceiling(shopPrice * (1 + (decimal)rules.OrderMarkup)) + rules.OrderDeliveryFee;

        public static double ToCrypto (long dollars, PlayerState p) => p.CryptoRate <= 0 ? 0 : dollars / p.CryptoRate;

        // One step of the daily random walk, clamped to the configured band.
        public static void UpdateCryptoRate (PlayerState p, EconomyRules rules, IRandom rng)
        {
            if (p.CryptoRate <= 0) p.CryptoRate = rules.CryptoStartRate;
            double change = (rng.NextDouble() * 2 - 1) * rules.CryptoDailyVolatility;
            p.CryptoRate = Math.Max(rules.CryptoMinRate, Math.Min(rules.CryptoMaxRate, p.CryptoRate * (1 + change)));
        }

        // Crypto -> cash at the current rate minus a laundering fee (better rep, lower fee).
        public static long Launder (PlayerState p, double coins, double feeFraction)
        {
            coins = Math.Min(coins, p.Crypto);
            if (coins <= 0) return 0;
            long cash = (long)Math.Floor(coins * p.CryptoRate * (1 - feeFraction));
            p.Crypto -= coins;
            p.Cash += cash;
            return cash;
        }
    }
}
