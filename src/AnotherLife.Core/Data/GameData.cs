using System.Collections.Generic;

namespace AnotherLife.Core.Data
{
    // Everything loaded from the data folder. Edit the JSON, press F9 in game to reload.
    public sealed class GameData
    {
        public RulesConfig Rules = new RulesConfig();
        public ChecksData Checks = new ChecksData();
        public Dictionary<string, ItemDef> Items = new Dictionary<string, ItemDef>();
        public List<LocationDef> Locations = new List<LocationDef>();
        public List<CrashPadDef> CrashPads = new List<CrashPadDef>();
        public List<PostalDef> Postals = new List<PostalDef>();

        public ItemDef Item (string id) => id != null && Items.TryGetValue(id, out var def) ? def : null;
    }

    public sealed class RulesConfig
    {
        public StartRules Start = new StartRules();
        public EconomyRules Economy = new EconomyRules();
        public DeathRules Death = new DeathRules();
        public ArrestRules Arrest = new ArrestRules();
        public DiceRules Dice = new DiceRules();
        public SkillRules Skills = new SkillRules();
        public DumpRules StolenCarDump = new DumpRules();
        public HudRules Hud = new HudRules();
    }

    public sealed class HudRules
    {
        // Shows "4021 | Elgin Ave, Downtown" beside the minimap.
        public bool ShowAddress = true;
    }

    public sealed class StartRules
    {
        public long Cash = 40;
        public List<ItemGrant> Items = new List<ItemGrant>();
    }

    public sealed class ItemGrant
    {
        public string Item;
        public int Count = 1;
    }

    public sealed class EconomyRules
    {
        // Bank can go this far negative before purchases are refused and collectors start.
        public long BankOverdraftLimit = -5000;
        // Collectors keep coming until the balance climbs back to this.
        public long CollectorsStopAtBalance = 0;
        public double CryptoStartRate = 250;
        public double CryptoDailyVolatility = 0.08;
        public double CryptoMinRate = 60;
        public double CryptoMaxRate = 900;
        public double LaunderFeeMin = 0.15;
        public double LaunderFeeMax = 0.35;
        // Phone orders: flat delivery fee plus markup on the shop price.
        public long OrderDeliveryFee = 15;
        public double OrderMarkup = 0.10;
    }

    public sealed class DeathRules
    {
        public long HospitalBill = 250;
        public string RespawnTag = "service.medical";
    }

    public sealed class ArrestRules
    {
        public long BookingFee = 100;
        public long ImpoundFee = 500;
        public long BondsmanCap = 1000;
        public int CopRepPerArrest = -10;
        public int CopRepFlaggedIdPenalty = -25;
        // At or below this cop rep, the player always has at least one wanted star.
        public int PermanentStarCopRep = -60;
        public string RespawnTag = "service.police";
    }

    public sealed class DiceRules
    {
        public bool Nat20AlwaysSucceeds = true;
        public bool Nat1AlwaysFails = true;
        public bool ShowDcOnScreen = true;
    }

    public sealed class SkillRules
    {
        public List<string> Skills = new List<string>();
        // XP needed for modifier +1, +2, ... (cumulative).
        public List<int> LevelThresholds = new List<int> { 60, 150, 300, 550, 900 };
        public double SuccessXpPerDc = 1.0;
        // Practice counts: a failed attempt still earns this fraction of the success XP.
        public double FailXpFraction = 0.34;
    }

    public sealed class DumpRules
    {
        public double CryptoMin = 0.02;
        public double CryptoMax = 0.08;
        public int CooldownMinutes = 45;
        public string LocationTag = "dump.stolen_car";
    }

    public sealed class ChecksData
    {
        public List<CheckActionDef> Actions = new List<CheckActionDef>();
        // GTA vehicle class id -> DC offset (sports +3, super +6 ...).
        public Dictionary<int, int> VehicleClassDcOffset = new Dictionary<int, int>();
        public List<ValueBump> ValueBumps = new List<ValueBump>();

        public CheckActionDef Action (string id) => Actions.Find(a => a.Id == id);
    }

    public sealed class CheckActionDef
    {
        public string Id;
        public string Label;
        public string Skill;
        public int BaseDc = 10;
        // Items that count as the tool for this action, best first.
        public List<string> Tools = new List<string>();
        public bool RequiresTool;
        public bool ToolGivesAdvantage;
        public bool VehicleScaled;
        public int NatOneWantedStars;
        public bool NatOneBreaksTool;
    }

    public sealed class ValueBump
    {
        public long MinValue;
        public int Add;
    }

    public sealed class ItemDef
    {
        public string Id;
        public string Label;
        public string Category;
        public int MaxStack = 1;
        public bool Illegal;
        public bool KeepOnDeath;
        public bool KeepOnArrest;
    }

    public sealed class LocationDef
    {
        public string Id;
        public string Label;
        public float X, Y, Z, Heading;
        public List<string> Tags = new List<string>();
        public bool Verify;

        public bool HasTag (string tag) => Tags != null && Tags.Contains(tag);
    }

    public sealed class CrashPadDef
    {
        public string Id;
        public string Label;
        public string Area;
        public string Perk;
        public string Drawback;
        public long WeeklyRent;
        public double HeatDecayMultiplier = 1.0;
        public Point Door = new Point();
        public Point Bed = new Point();
        public Point Workbench = new Point();
        public Point Stash = new Point();
        public Point VehicleSpawn = new Point();
        public bool Verify;
    }

    // Postal code points from DevBlocky/nearest-postal (new-postals.json, MIT).
    // A position's postal is the code of the closest point.
    public sealed class PostalDef
    {
        public string Code;
        public float X, Y;
    }

    public sealed class Point
    {
        public float X, Y, Z, Heading;
    }
}
