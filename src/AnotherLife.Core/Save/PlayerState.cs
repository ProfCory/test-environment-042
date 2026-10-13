using System.Collections.Generic;
using AnotherLife.Core.Data;

namespace AnotherLife.Core
{
    // Everything that persists between sessions. Serialized as-is to save/slot1.json.
    public sealed class PlayerState
    {
        public const int CurrentVersion = 1;

        public int SaveVersion = CurrentVersion;
        public bool OnboardingDone;
        public CharacterState Character = new CharacterState();
        public string CrashPadId;

        public long Cash;
        public long Bank;
        public bool HasBankAccount;
        public double Crypto;
        public double CryptoRate;
        public bool CollectorsActive;
        // False until the game wallet has been seeded from this save (new game) once.
        public bool WalletSynced;
        public long BondDebt;

        public List<ItemStack> Inventory = new List<ItemStack>();
        public Dictionary<string, int> SkillXp = new Dictionary<string, int>();
        public bool Momentum;

        public bool FakeIdFlagged;
        public int CopRep;
        public List<Impound> Impounds = new List<Impound>();

        // Game-minute timestamps (see GameMinutes) at which a cooldown ends.
        public Dictionary<string, long> Cooldowns = new Dictionary<string, long>();
        public long GameMinutes;

        public Point LastPosition;
    }

    public sealed class CharacterState
    {
        public string Name = "";
        public bool Female;
        public int FacePreset;
        public int Hair;
        public int HairColor;
        public int EyeColor;
    }

    public sealed class ItemStack
    {
        public string Item;
        public int Count = 1;
        // Free-form tag, e.g. the plate a key belongs to.
        public string Meta;
    }

    public sealed class Impound
    {
        public string Plate;
        public long Fee;
        public string LocationId;
    }
}
