using System;
using System.IO;
using System.Windows.Forms;
using AnotherLife.Core;
using AnotherLife.Core.Data;
using AnotherLife.Core.Rules;
using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using LemonUI;

namespace AnotherLife.InGame
{
    // Entry point. ScriptHookVDotNet creates this once when the game loads the scripts folder.
    public sealed class Main : Script
    {
        const Keys ReloadKey = Keys.F9;
        const Keys MenuKey = Keys.F10;
        const int AutosaveMs = 5 * 60 * 1000;
        const int RealMsPerGameMinute = 2000;   // GTA's clock: one in-game minute every 2 real seconds
        const int GameMinutesPerDay = 24 * 60;
        const int RespawnTimeoutMs = 60 * 1000;

        // PED_TYPE values that count as police for "killed by cops".
        static readonly int[] CopPedTypes = { 6, 27, 29 };

        internal readonly Paths Paths;
        internal GameData Data;
        internal PlayerState Player;
        internal readonly IRandom Rng = new SystemRandom();

        readonly ObjectPool pool = new ObjectPool();
        readonly DebugMenu menu;
        internal readonly AddressHud Address;
        internal readonly CashBridge Cash;
        readonly Terminals terminals;
        int stripWeaponsUntil;

        bool wasDead;
        bool wasArrested;
        LocationDef pendingRespawn;
        int pendingSince;
        bool sawFadeOut;
        int lastTick;
        int msSinceAutosave;
        int msTowardGameMinute;
        int msSinceStarCheck;

        public Main ()
        {
            Paths = new Paths(BaseDirectory);
            Log.Init(Paths.Log);

            LoadData(announce: false);
            LoadOrCreateSave();

            Address = new AddressHud(this);
            Cash = new CashBridge(this);
            terminals = new Terminals(this);
            menu = new DebugMenu(this);
            pool.Add(menu.Root);
            foreach (var sub in menu.SubMenus) pool.Add(sub);
            pool.Add(terminals.AtmMenu);
            pool.Add(terminals.PayphoneMenu);

            Tick += OnTick;
            KeyDown += OnKeyDown;
            Aborted += (s, e) => SaveNow(quiet: true);

            Notification.Show($"~y~Another Life~s~ loaded. ~b~{MenuKey}~s~ menu, ~b~{ReloadKey}~s~ reload data.");
        }

        internal void LoadData (bool announce)
        {
            var result = DataLoader.Load(Paths.Data);
            Data = result.Data;
            foreach (var error in result.Errors)
            {
                Log.Write("data: " + error);
                Notification.Show("~r~Data error~s~: " + error);
            }
            if (announce && result.Ok) Notification.Show("Data reloaded.");
        }

        void LoadOrCreateSave ()
        {
            Player = SaveStore.Load(Paths.Save, out var error);
            if (error != null)
            {
                Log.Write("save: " + error);
                Notification.Show("~r~Save was corrupt~s~, a copy was kept. Starting fresh.");
            }
            if (Player == null)
            {
                Player = SaveStore.NewGame(Data);
                Log.Write("new game");
            }
        }

        internal void NewGame ()
        {
            Player = SaveStore.NewGame(Data);
            Cash.Pull(); // seeds the wallet with the starting $40
            SaveNow(quiet: false);
        }

        // Payphone fee: best rate for now; reputation moves it toward the max in M7.
        internal double PayphoneFee () => Data.Rules.Economy.PayphoneFeeMin;

        internal void SaveNow (bool quiet)
        {
            try
            {
                var pos = Game.Player.Character.Position;
                Player.LastPosition = new Point { X = pos.X, Y = pos.Y, Z = pos.Z, Heading = Game.Player.Character.Heading };
                SaveStore.Save(Paths.Save, Player);
                msSinceAutosave = 0;
                if (!quiet) Notification.Show("Saved.");
            }
            catch (Exception ex)
            {
                Log.Write("save failed: " + ex);
                Notification.Show("~r~Save failed~s~ (see AnotherLife/log.txt)");
            }
        }

        void OnKeyDown (object sender, KeyEventArgs e)
        {
            if (e.KeyCode == ReloadKey) LoadData(announce: true);
            else if (e.KeyCode == MenuKey) menu.Toggle();
        }

        void OnTick (object sender, EventArgs e)
        {
            pool.Process();

            int now = Game.GameTime;
            int delta = lastTick == 0 ? 0 : Math.Min(now - lastTick, 1000);
            lastTick = now;

            Cash.Tick(delta);
            Address.Tick(delta);
            terminals.Tick(delta);
            StripWeaponsIfPending();
            AdvanceClock(delta);
            WatchDeathAndArrest();
            EnforcePermanentStar(delta);

            msSinceAutosave += delta;
            if (msSinceAutosave >= AutosaveMs) SaveNow(quiet: true);
        }

        void AdvanceClock (int deltaMs)
        {
            msTowardGameMinute += deltaMs;
            while (msTowardGameMinute >= RealMsPerGameMinute)
            {
                msTowardGameMinute -= RealMsPerGameMinute;
                Player.GameMinutes++;
                if (Player.GameMinutes % GameMinutesPerDay == 0) OnNewDay();
            }
        }

        void OnNewDay ()
        {
            Economy.UpdateCryptoRate(Player, Data.Rules.Economy, Rng);
        }

        void WatchDeathAndArrest ()
        {
            var ped = Game.Player.Character;
            bool dead = ped.IsDead;
            bool arrested = Function.Call<bool>(Hash.IS_PLAYER_BEING_ARRESTED, Game.Player.Handle, true);

            if (dead && !wasDead)
            {
                var killer = ped.Killer as Ped;
                bool byCops = killer != null && Array.IndexOf(CopPedTypes, Function.Call<int>(Hash.GET_PED_TYPE, killer.Handle)) >= 0;
                if (byCops) OnArrested(ped.Position);
                else OnDied(ped.Position);
            }
            else if (arrested && !wasArrested && !dead)
            {
                OnArrested(ped.Position);
            }

            // Wait for the game's own Wasted/Busted sequence to fade out, respawn the player
            // and start fading back in; moving earlier gets overridden by the game's respawn.
            if (pendingRespawn != null)
            {
                if (GTA.UI.Screen.IsFadedOut) sawFadeOut = true;
                bool back = !dead && !arrested && (GTA.UI.Screen.IsFadingIn || GTA.UI.Screen.IsFadedIn);
                if (sawFadeOut && back)
                {
                    ped.Position = new Vector3(pendingRespawn.X, pendingRespawn.Y, pendingRespawn.Z);
                    ped.Heading = pendingRespawn.Heading;
                    pendingRespawn = null;
                }
                else if (Game.GameTime - pendingSince > RespawnTimeoutMs)
                {
                    Log.Write("respawn relocation timed out");
                    pendingRespawn = null;
                }
            }

            wasDead = dead;
            wasArrested = arrested;
        }

        void OnDied (Vector3 at)
        {
            Cash.Pull();
            var r = Penalties.ApplyDeath(Player, Data, at.X, at.Y, at.Z);
            Cash.Push();
            ExpectRespawn(r.Respawn);
            if (Data.Rules.Death.LoseWeapons) StripWeapons();
            Texts.Death(r);
            Log.Write($"death: bill={r.Bill} cashLost={r.CashLost} crypto={r.CryptoLost} items={r.ItemsLost.Count}");
            SaveNow(quiet: true);
        }

        void OnArrested (Vector3 at)
        {
            // Owned-vehicle tracking arrives in M4; until then nothing is impounded.
            Cash.Pull();
            var r = Penalties.ApplyArrest(Player, Data, at.X, at.Y, at.Z, drivingOwnedPlate: null);
            Cash.Push();
            ExpectRespawn(r.Respawn);
            if (Data.Rules.Arrest.LoseWeapons) StripWeapons();
            Texts.Arrest(r);
            Log.Write($"arrest: cash={r.CashSeized} fee={r.Fee} bond={r.BondTaken} noBail={r.NoBail} star={r.PermanentStar}");
            SaveNow(quiet: true);
        }

        // Freemode Identity restores its last weapon snapshot when it re-applies your look after a
        // respawn, so keep clearing weapons for a while until its snapshot catches up.
        void StripWeapons () => stripWeaponsUntil = Game.GameTime + RespawnTimeoutMs + 10000;

        void StripWeaponsIfPending ()
        {
            if (stripWeaponsUntil == 0) return;
            if (Game.GameTime > stripWeaponsUntil) { stripWeaponsUntil = 0; return; }
            var ped = Game.Player.Character;
            if (!ped.IsDead) ped.Weapons.RemoveAll();
        }

        void ExpectRespawn (LocationDef at)
        {
            pendingRespawn = at;
            pendingSince = Game.GameTime;
            sawFadeOut = false;
        }

        void EnforcePermanentStar (int deltaMs)
        {
            msSinceStarCheck += deltaMs;
            if (msSinceStarCheck < 1000) return;
            msSinceStarCheck = 0;
            if (Game.Player.WantedLevel == 0 && Penalties.HasPermanentStar(Player, Data.Rules.Arrest))
            {
                Game.Player.WantedLevel = 1;
            }
        }

        // Test a d20 check from the menu, using the current vehicle for class scaling.
        internal CheckOutcome RunCheck (string actionId)
        {
            var def = Data.Checks.Action(actionId);
            var ctx = new CheckContext { Wanted = Game.Player.WantedLevel > 0 };
            var vehicle = Game.Player.Character.CurrentVehicle;
            if (vehicle != null) ctx.VehicleClass = (int)vehicle.ClassType;
            if (def != null) Checks.ForPlayer(def, Player, ctx);

            var o = Checks.Attempt(actionId, Data, Player, ctx, Rng);
            if (o.WantedStarsAdded > 0) Game.Player.WantedLevel = Math.Min(5, Game.Player.WantedLevel + o.WantedStarsAdded);
            Texts.Check(o, Data.Rules.Dice.ShowDcOnScreen);
            return o;
        }

        internal void LogPosition (string label)
        {
            var ped = Game.Player.Character;
            var p = ped.Position;
            string line = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "{0:yyyy-MM-dd HH:mm}  {1}  \"x\": {2:0.0}, \"y\": {3:0.0}, \"z\": {4:0.0}, \"heading\": {5:0}  ({6})",
                DateTime.Now, label, p.X, p.Y, p.Z, ped.Heading, Address.AddressOf(p));
            File.AppendAllText(Paths.Positions, line + Environment.NewLine);
            Notification.Show("Logged: " + label);
        }

        internal void Teleport (Point to)
        {
            var ped = Game.Player.Character;
            Entity target = ped.CurrentVehicle != null ? (Entity)ped.CurrentVehicle : ped;
            target.Position = new Vector3(to.X, to.Y, to.Z);
            target.Heading = to.Heading;
        }
    }

    internal sealed class Paths
    {
        public readonly string Root, Data, Save, Log, Positions;

        public Paths (string scriptsDir)
        {
            Root = Path.Combine(scriptsDir, "AnotherLife");
            Data = Path.Combine(Root, "data");
            Save = Path.Combine(Root, "save", "slot1.json");
            Log = Path.Combine(Root, "log.txt");
            Positions = Path.Combine(Root, "positions.txt");
        }
    }

    internal static class Log
    {
        static string path;

        public static void Init (string file)
        {
            path = file;
            Directory.CreateDirectory(Path.GetDirectoryName(file));
        }

        public static void Write (string message)
        {
            try { File.AppendAllText(path, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  {message}{Environment.NewLine}"); }
            catch { /* logging must never crash the game */ }
        }
    }
}
