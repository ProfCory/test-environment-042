using System;
using System.Collections.Generic;
using AnotherLife.Core.Data;

namespace AnotherLife.Core.Rules
{
    public enum BillPayment { Cash, Bank, PartialCash, Bond, Waived }

    public sealed class DeathResult
    {
        public BillPayment Bill;
        public long BillAmount;
        public long CashLost;
        public double CryptoLost;
        public List<ItemStack> ItemsLost = new List<ItemStack>();
        public LocationDef Respawn;
    }

    public sealed class ArrestResult
    {
        public long CashSeized;
        public double CryptoSeized;
        public List<ItemStack> ItemsSeized = new List<ItemStack>();
        public bool FakeIdFlagged;
        public BillPayment Fee;
        public long FeeAmount;
        public bool BondTaken;
        public bool NoBail;
        public Impound Impounded;
        public bool PermanentStar;
        public LocationDef Respawn;
    }

    public static class Penalties
    {
        // Hospital bill comes out of carried cash if it covers it, otherwise goes on the card.
        // With neither, the hospital takes what you have and writes off the rest.
        // Then you lose everything carried except items marked keepOnDeath (card, ID, keys).
        public static DeathResult ApplyDeath (PlayerState p, GameData data, float x, float y, float z)
        {
            var rules = data.Rules;
            var r = new DeathResult { BillAmount = rules.Death.HospitalBill };

            if (p.Cash >= r.BillAmount)
            {
                p.Cash -= r.BillAmount;
                r.Bill = BillPayment.Cash;
            }
            else if (Economy.CanUseCard(p))
            {
                Economy.ChargeBankForced(p, r.BillAmount, rules.Economy);
                r.Bill = BillPayment.Bank;
            }
            else
            {
                r.Bill = p.Cash > 0 ? BillPayment.PartialCash : BillPayment.Waived;
                p.Cash = 0;
            }

            r.CashLost = p.Cash;
            r.CryptoLost = p.Crypto;
            p.Cash = 0;
            p.Crypto = 0;
            r.ItemsLost = Inventory.Strip(p, def => def.KeepOnDeath, data);
            p.Momentum = false;
            r.Respawn = Nearest(data.Locations, rules.Death.RespawnTag, x, y, z);
            return r;
        }

        // Arrested, or killed by police. Everything carried is seized except keepOnArrest items;
        // the fake ID stays with you but is flagged. Booking fee goes on the card, or a bail
        // bondsman covers it up to the bond cap. Past the cap there's no bail: you serve time.
        public static ArrestResult ApplyArrest (PlayerState p, GameData data, float x, float y, float z, string drivingOwnedPlate)
        {
            var rules = data.Rules;
            var arrest = rules.Arrest;
            var r = new ArrestResult
            {
                FeeAmount = arrest.BookingFee,
                Respawn = Nearest(data.Locations, arrest.RespawnTag, x, y, z)
            };

            r.CashSeized = p.Cash;
            r.CryptoSeized = p.Crypto;
            p.Cash = 0;
            p.Crypto = 0;
            r.ItemsSeized = Inventory.Strip(p, def => def.KeepOnArrest, data);
            p.Momentum = false;

            if (Inventory.Has(p, Inventory.FakeId))
            {
                p.FakeIdFlagged = true;
                r.FakeIdFlagged = true;
            }
            p.CopRep += arrest.CopRepPerArrest;

            if (Economy.CanUseCard(p) && p.Bank - r.FeeAmount >= rules.Economy.BankOverdraftLimit)
            {
                Economy.ChargeBankForced(p, r.FeeAmount, rules.Economy);
                r.Fee = BillPayment.Bank;
            }
            else if (p.BondDebt + r.FeeAmount <= arrest.BondsmanCap)
            {
                p.BondDebt += r.FeeAmount;
                r.Fee = BillPayment.Bond;
                r.BondTaken = true;
            }
            else
            {
                r.Fee = BillPayment.Waived;
                r.NoBail = true;
            }

            if (!string.IsNullOrEmpty(drivingOwnedPlate))
            {
                r.Impounded = new Impound { Plate = drivingOwnedPlate, Fee = arrest.ImpoundFee, LocationId = r.Respawn?.Id };
                p.Impounds.RemoveAll(i => i.Plate == drivingOwnedPlate);
                p.Impounds.Add(r.Impounded);
            }

            r.PermanentStar = HasPermanentStar(p, arrest);
            return r;
        }

        // A flagged fake ID drags your standing with every cop until it's replaced.
        public static int EffectiveCopRep (PlayerState p, ArrestRules rules) =>
            p.CopRep + (p.FakeIdFlagged ? rules.CopRepFlaggedIdPenalty : 0);

        public static bool HasPermanentStar (PlayerState p, ArrestRules rules) =>
            EffectiveCopRep(p, rules) <= rules.PermanentStarCopRep;

        public static LocationDef Nearest (List<LocationDef> locations, string tag, float x, float y, float z)
        {
            LocationDef best = null;
            double bestDist = double.MaxValue;
            foreach (var l in locations)
            {
                if (!l.HasTag(tag)) continue;
                double d = Math.Pow(l.X - x, 2) + Math.Pow(l.Y - y, 2) + Math.Pow(l.Z - z, 2);
                if (d < bestDist) { bestDist = d; best = l; }
            }
            return best;
        }
    }
}
