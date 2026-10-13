using System.Collections.Generic;
using AnotherLife.Core.Rules;
using GTA;
using GTA.UI;
using LemonUI.Menus;

namespace AnotherLife.InGame
{
    // F10 test bench for M0: inspect state, poke money, roll checks, simulate penalties,
    // and capture coordinates for the data files.
    internal sealed class DebugMenu
    {
        public readonly NativeMenu Root = new NativeMenu("Another Life", "Debug (M0)");
        public readonly List<NativeMenu> SubMenus = new List<NativeMenu>();

        readonly Main main;
        readonly NativeItem status = new NativeItem("Status");
        readonly NativeMenu checks = new NativeMenu("Another Life", "Roll a check");
        readonly NativeMenu teleports = new NativeMenu("Another Life", "Go to (verify coords)");

        public DebugMenu (Main main)
        {
            this.main = main;
            SubMenus.Add(checks);
            SubMenus.Add(teleports);

            Root.Add(status);
            Add(Root, "Text me the controls", Texts.Controls);
            Add(Root, "+$100 cash", () => main.Player.Cash += 100);
            Add(Root, "+$1,000 bank (opens account, gives card)", () =>
            {
                main.Player.HasBankAccount = true;
                if (!Inventory.Has(main.Player, Inventory.BankCard)) Inventory.Add(main.Player, Inventory.BankCard);
                Economy.Deposit(main.Player, 1000, main.Data.Rules.Economy);
            });
            Add(Root, "-$1,000 bank (test collectors)", () => Economy.ChargeBankForced(main.Player, 1000, main.Data.Rules.Economy));
            Add(Root, "+0.1 crypto", () => main.Player.Crypto += 0.1);
            Add(Root, "Add lockpick + screwdriver", () =>
            {
                Inventory.Add(main.Player, "lockpick");
                Inventory.Add(main.Player, "screwdriver");
            });
            Root.AddSubMenu(checks, "Roll a check");
            Add(Root, "Simulate death penalty", () => Texts.Death(Penalties.ApplyDeath(main.Player, main.Data, Pos.X, Pos.Y, Pos.Z)));
            Add(Root, "Simulate arrest penalty", () => Texts.Arrest(Penalties.ApplyArrest(main.Player, main.Data, Pos.X, Pos.Y, Pos.Z, null)));
            Add(Root, "Clear fake ID flag", () => main.Player.FakeIdFlagged = false);
            Add(Root, "Where am I?", () => Notification.Show(main.Address.AddressOf(Pos)));
            Add(Root, "GPS to postal", () =>
            {
                var code = Game.GetUserInput("postal");
                if (string.IsNullOrWhiteSpace(code)) return;
                Notification.Show(main.Address.RouteTo(code) ? $"Route set to #{code.Trim().TrimStart('#')}." : $"~r~No postal {code}.");
            });
            Add(Root, "Log my position", () => main.LogPosition(Game.GetUserInput("label") ?? "spot"));
            Root.AddSubMenu(teleports, "Go to (verify coords)");
            Add(Root, "Reload data (F9)", () => main.LoadData(announce: true));
            Add(Root, "Save now", () => main.SaveNow(quiet: false));
            Add(Root, "~r~Start a new game~s~ (wipes save)", () => main.NewGame());

            Root.Shown += (s, e) => Rebuild();
        }

        static GTA.Math.Vector3 Pos => Game.Player.Character.Position;

        public void Toggle ()
        {
            Root.Visible = !Root.Visible;
        }

        void Rebuild ()
        {
            var p = main.Player;
            status.Title = $"${p.Cash} cash | ${p.Bank} bank | {p.Crypto:0.###} crypto";
            status.Description =
                $"Crypto rate ${p.CryptoRate:0}. Bond debt ${p.BondDebt}. Collectors: {(p.CollectorsActive ? "ON" : "off")}. " +
                $"Cop rep {p.CopRep}{(p.FakeIdFlagged ? " (ID flagged)" : "")}. Items: {p.Inventory.Count} stacks. Day {p.GameMinutes / 1440 + 1}.";

            checks.Clear();
            foreach (var action in main.Data.Checks.Actions)
            {
                var id = action.Id;
                int mod = Skills.Modifier(p, action.Skill, main.Data.Rules.Skills);
                Add(checks, $"{action.Label} (DC {action.BaseDc}, {action.Skill} +{mod})", () => main.RunCheck(id));
            }

            teleports.Clear();
            foreach (var pad in main.Data.CrashPads)
            {
                var door = pad.Door;
                Add(teleports, $"Crash pad: {pad.Label}{(pad.Verify ? " *" : "")}", () => main.Teleport(door));
            }
            foreach (var loc in main.Data.Locations)
            {
                var to = new Core.Data.Point { X = loc.X, Y = loc.Y, Z = loc.Z, Heading = loc.Heading };
                Add(teleports, $"{loc.Label}{(loc.Verify ? " *" : "")}", () => main.Teleport(to));
            }
        }

        void Add (NativeMenu menu, string title, System.Action action)
        {
            var item = new NativeItem(title);
            item.Activated += (s, e) =>
            {
                main.Cash.Pull();
                action();
                main.Cash.Push();
                if (menu == Root) Rebuild();
            };
            menu.Add(item);
        }
    }
}
