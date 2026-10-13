using GTA;

namespace AnotherLife.InGame
{
    // Keeps the mod's cash equal to the game wallet. With Freemode Identity spoofing, Game.Player.Money
    // is redirected to its wallet, so vanilla shops, job mods and this mod all spend the same cash.
    // Call Pull() before changing Player.Cash and Push() after.
    internal sealed class CashBridge
    {
        const int PollMs = 500;

        readonly Main main;
        int sincePoll = PollMs;

        public CashBridge (Main main) { this.main = main; }

        static readonly Model MaleFreemode = new Model("mp_m_freemode_01");
        static readonly Model FemaleFreemode = new Model("mp_f_freemode_01");

        // Only while playing the freemode character: as Michael/Franklin/Trevor, Game.Player.Money
        // is that protagonist's real cash, which this mod must never adopt or overwrite.
        public bool Active
        {
            get
            {
                if (!main.Data.Rules.Economy.CashFromGameWallet) return false;
                var model = Game.Player.Character.Model;
                return model == MaleFreemode || model == FemaleFreemode;
            }
        }

        public void Pull ()
        {
            if (!Active) return;
            if (!main.Player.WalletSynced) { Seed(); return; }
            main.Player.Cash = Game.Player.Money;
        }

        public void Push ()
        {
            if (!Active) return;
            Game.Player.Money = (int)System.Math.Min(main.Player.Cash, int.MaxValue);
            main.Player.WalletSynced = true;
        }

        // A new save's starting cash overwrites whatever the wallet held.
        void Seed ()
        {
            Push();
            Log.Write($"wallet seeded with ${main.Player.Cash}");
        }

        // Picks up shop purchases, pickups and job payouts made outside this mod.
        public void Tick (int deltaMs)
        {
            sincePoll += deltaMs;
            if (sincePoll < PollMs) return;
            sincePoll = 0;
            Pull();
        }
    }
}
