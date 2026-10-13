using System.Collections.Generic;
using System.Linq;
using AnotherLife.Core.Rules;
using GTA;
using GTA.UI;
using LemonUI.Menus;

namespace AnotherLife.InGame
{
    // Every ATM and payphone prop in the world is usable: walk up, press E.
    //   ATM:      cash -> bank (free), bank -> cash (flat fee). Needs a bank account and the card.
    //   Payphone: crypto -> bank at today's rate, minus a fee. Needs a bank account.
    internal sealed class Terminals
    {
        const int ScanMs = 250;

        public readonly NativeMenu AtmMenu = new NativeMenu("ATM", "Maze Bank");
        public readonly NativeMenu PayphoneMenu = new NativeMenu("Payphone", "Wire crypto to your bank");

        readonly Main main;
        Model[] atmModels = new Model[0];
        Model[] payphoneModels = new Model[0];
        object modelsFrom;
        int sinceScan = ScanMs;
        Prop nearAtm, nearPayphone;

        public Terminals (Main main) { this.main = main; }

        bool AnyOpen => AtmMenu.Visible || PayphoneMenu.Visible;

        public void Tick (int deltaMs)
        {
            RefreshModels();
            var ped = Game.Player.Character;
            if (ped.IsInVehicle() || ped.IsDead) { nearAtm = nearPayphone = null; return; }

            sinceScan += deltaMs;
            if (sinceScan >= ScanMs)
            {
                sinceScan = 0;
                float range = main.Data.Rules.Interactions.Range;
                nearAtm = Closest(World.GetNearbyProps(ped.Position, range, atmModels));
                nearPayphone = nearAtm == null ? Closest(World.GetNearbyProps(ped.Position, range, payphoneModels)) : null;
            }

            if (AnyOpen || (nearAtm == null && nearPayphone == null)) return;

            Screen.ShowHelpTextThisFrame(nearAtm != null ? "Press ~INPUT_CONTEXT~ to use the ATM." : "Press ~INPUT_CONTEXT~ to use the payphone.");
            if (Game.IsControlJustPressed(Control.Context))
            {
                if (nearAtm != null) OpenAtm();
                else OpenPayphone();
            }
        }

        // Rebuild the model list only when data is (re)loaded.
        void RefreshModels ()
        {
            var rules = main.Data.Rules.Interactions;
            if (ReferenceEquals(modelsFrom, rules)) return;
            modelsFrom = rules;
            atmModels = rules.AtmModels.Select(m => new Model(m)).ToArray();
            payphoneModels = rules.PayphoneModels.Select(m => new Model(m)).ToArray();
        }

        static Prop Closest (IEnumerable<Prop> props) =>
            props.OrderBy(p => p.Position.DistanceToSquared(Game.Player.Character.Position)).FirstOrDefault();

        void OpenAtm ()
        {
            main.Cash.Pull();
            var p = main.Player;
            var rules = main.Data.Rules.Economy;
            AtmMenu.Clear();

            if (!p.HasBankAccount)
            {
                AtmMenu.Name = "No account";
                AtmMenu.Add(new NativeItem("You don't have an account.", "Bank accounts open at Verified (T2)."));
                AtmMenu.Visible = true;
                return;
            }
            if (!Inventory.Has(p, Inventory.BankCard))
            {
                AtmMenu.Name = "No card";
                AtmMenu.Add(new NativeItem("You need your bank card."));
                AtmMenu.Visible = true;
                return;
            }

            AtmMenu.Name = $"Bank ${p.Bank:N0}  |  Cash ${p.Cash:N0}";
            Item(AtmMenu, "Deposit all cash", () => Deposit(p.Cash));
            foreach (var amount in new long[] { 20, 100, 500 }) Item(AtmMenu, $"Deposit ${amount}", () => Deposit(amount));
            Item(AtmMenu, "Deposit amount...", () => Deposit(AskAmount()));
            foreach (var amount in new long[] { 20, 100, 500 }) Item(AtmMenu, $"Withdraw ${amount} (+${rules.AtmWithdrawFee} fee)", () => Withdraw(amount));
            Item(AtmMenu, $"Withdraw amount... (+${rules.AtmWithdrawFee} fee)", () => Withdraw(AskAmount()));
            AtmMenu.Visible = true;
        }

        void Deposit (long amount)
        {
            main.Cash.Pull();
            if (Economy.AtmDeposit(main.Player, amount, main.Data.Rules.Economy))
            {
                main.Cash.Push();
                Notification.Show($"Deposited ${amount:N0}. Balance ${main.Player.Bank:N0}.");
                Done(AtmMenu);
            }
            else Notification.Show("~r~Not enough cash.");
        }

        void Withdraw (long amount)
        {
            main.Cash.Pull();
            if (Economy.AtmWithdraw(main.Player, amount, main.Data.Rules.Economy))
            {
                main.Cash.Push();
                Notification.Show($"Withdrew ${amount:N0} (${main.Data.Rules.Economy.AtmWithdrawFee} fee). Balance ${main.Player.Bank:N0}.");
                Done(AtmMenu);
            }
            else Notification.Show("~r~Insufficient funds.");
        }

        void OpenPayphone ()
        {
            var p = main.Player;
            double fee = main.PayphoneFee();
            PayphoneMenu.Clear();
            PayphoneMenu.Name = $"{p.Crypto:0.####} coin @ ${p.CryptoRate:0} | fee {fee:P0}";

            if (!p.HasBankAccount)
            {
                PayphoneMenu.Add(new NativeItem("\"No account, no wire. Call me when you're legit.\""));
                PayphoneMenu.Visible = true;
                return;
            }

            Item(PayphoneMenu, $"Wire all (~${(long)(p.Crypto * p.CryptoRate * (1 - fee)):N0} to bank)", () => Wire(p.Crypto, fee));
            Item(PayphoneMenu, "Wire amount (coins)...", () =>
            {
                var input = Game.GetUserInput("0.1");
                if (double.TryParse(input, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var coins))
                    Wire(coins, fee);
            });
            PayphoneMenu.Visible = true;
        }

        void Wire (double coins, double fee)
        {
            long dollars = Economy.PayphoneWire(main.Player, coins, fee, main.Data.Rules.Economy);
            if (dollars > 0)
            {
                Notification.Show($"Wired ${dollars:N0} to your bank. Balance ${main.Player.Bank:N0}.");
                Done(PayphoneMenu);
            }
            else Notification.Show("~r~Nothing to wire.");
        }

        static long AskAmount ()
        {
            var input = Game.GetUserInput("100");
            return long.TryParse(input, out var amount) ? amount : 0;
        }

        static void Item (NativeMenu menu, string title, System.Action action)
        {
            var item = new NativeItem(title);
            item.Activated += (s, e) => action();
            menu.Add(item);
        }

        static void Done (NativeMenu menu) => menu.Visible = false;
    }
}
