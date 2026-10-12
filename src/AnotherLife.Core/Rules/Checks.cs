using System;
using AnotherLife.Core.Data;

namespace AnotherLife.Core.Rules
{
    public interface IRandom
    {
        int Next (int minInclusive, int maxInclusive);
        double NextDouble ();
    }

    public sealed class SystemRandom : IRandom
    {
        readonly Random rng;
        public SystemRandom (int? seed = null) { rng = seed.HasValue ? new Random(seed.Value) : new Random(); }
        public int Next (int minInclusive, int maxInclusive) => rng.Next(minInclusive, maxInclusive + 1);
        public double NextDouble () => rng.NextDouble();
    }

    public enum RollMode { Normal, Advantage, Disadvantage }

    // What the world looks like when the player attempts the action.
    public sealed class CheckContext
    {
        public int? VehicleClass;
        public long? VehicleValue;
        public int DcAdjust;
        public bool HasTool;
        // The inventory item used as the tool; removed if it breaks.
        public string ToolItem;
        public bool Wanted;
        public RollMode? Force;
    }

    public sealed class CheckOutcome
    {
        public string ActionId;
        public string Label;
        public string Skill;
        public int Dc;
        public int Modifier;
        public RollMode Mode;
        public int RollA;
        public int RollB;      // 0 when only one die was rolled
        public int Natural;    // the die that counted
        public int Total;
        public bool Success;
        public bool Nat1;
        public bool Nat20;
        public int XpGained;
        public bool LeveledUp;
        public int NewLevel;
        public bool MomentumGained;
        public bool MomentumUsed;
        public bool ToolBroke;
        public string ToolItem;
        public int WantedStarsAdded;
        public string Error;
    }

    // d20 + skill modifier vs DC. Ported from ALN3's aln-minigame, with per-skill practice
    // replacing the single criminal level so each action gets easier the more you do it.
    public static class Checks
    {
        // The first listed tool the player is carrying, or null.
        public static string FindTool (CheckActionDef def, PlayerState player)
        {
            foreach (var tool in def.Tools)
            {
                if (Inventory.Has(player, tool)) return tool;
            }
            return null;
        }

        // Fills in tool and wanted state from the player, for callers that don't set them.
        public static CheckContext ForPlayer (CheckActionDef def, PlayerState player, CheckContext ctx)
        {
            ctx.ToolItem = ctx.ToolItem ?? FindTool(def, player);
            ctx.HasTool = ctx.HasTool || ctx.ToolItem != null;
            return ctx;
        }

        public static int ComputeDc (CheckActionDef def, ChecksData checks, CheckContext ctx)
        {
            int dc = def.BaseDc;
            if (def.VehicleScaled && ctx.VehicleClass.HasValue &&
                checks.VehicleClassDcOffset.TryGetValue(ctx.VehicleClass.Value, out var offset))
            {
                dc += offset;
            }
            if (ctx.VehicleValue.HasValue)
            {
                int bump = 0;
                foreach (var b in checks.ValueBumps)
                {
                    if (ctx.VehicleValue.Value >= b.MinValue) bump = Math.Max(bump, b.Add);
                }
                dc += bump;
            }
            return dc + ctx.DcAdjust;
        }

        // Advantage and disadvantage cancel out, as in 5e.
        public static RollMode ResolveMode (CheckActionDef def, CheckContext ctx, bool momentum)
        {
            if (ctx.Force.HasValue) return ctx.Force.Value;
            bool adv = momentum || (def.ToolGivesAdvantage && ctx.HasTool);
            bool dis = ctx.Wanted;
            if (adv == dis) return RollMode.Normal;
            return adv ? RollMode.Advantage : RollMode.Disadvantage;
        }

        public static CheckOutcome Attempt (string actionId, GameData data, PlayerState player, CheckContext ctx, IRandom rng)
        {
            var def = data.Checks.Action(actionId);
            if (def == null) return new CheckOutcome { ActionId = actionId, Error = "unknown_action" };
            if (def.RequiresTool && !ctx.HasTool) return new CheckOutcome { ActionId = actionId, Error = "tool_required" };

            bool momentum = player.Momentum && !ctx.Force.HasValue;
            var mode = ResolveMode(def, ctx, momentum);
            var rules = data.Rules;

            var o = new CheckOutcome
            {
                ActionId = def.Id,
                Label = def.Label,
                Skill = def.Skill,
                Dc = ComputeDc(def, data.Checks, ctx),
                Modifier = Skills.Modifier(player, def.Skill, rules.Skills),
                Mode = mode,
                MomentumUsed = momentum && mode == RollMode.Advantage
            };
            if (o.MomentumUsed) player.Momentum = false;

            o.RollA = rng.Next(1, 20);
            if (mode != RollMode.Normal) o.RollB = rng.Next(1, 20);
            o.Natural = mode == RollMode.Advantage ? Math.Max(o.RollA, o.RollB)
                      : mode == RollMode.Disadvantage ? Math.Min(o.RollA, o.RollB)
                      : o.RollA;
            o.Nat1 = o.Natural == 1;
            o.Nat20 = o.Natural == 20;
            o.Total = o.Natural + o.Modifier;

            o.Success = o.Total >= o.Dc;
            if (o.Nat20 && rules.Dice.Nat20AlwaysSucceeds) o.Success = true;
            if (o.Nat1 && rules.Dice.Nat1AlwaysFails) o.Success = false;

            if (o.Nat20 && !player.Momentum)
            {
                player.Momentum = true;
                o.MomentumGained = true;
            }
            if (o.Nat1)
            {
                o.WantedStarsAdded = def.NatOneWantedStars;
                o.ToolBroke = def.NatOneBreaksTool && ctx.HasTool;
                if (o.ToolBroke && ctx.ToolItem != null && Inventory.Remove(player, ctx.ToolItem)) o.ToolItem = ctx.ToolItem;
            }

            double xp = o.Dc * rules.Skills.SuccessXpPerDc;
            if (!o.Success) xp *= rules.Skills.FailXpFraction;
            o.XpGained = (int)Math.Round(xp);
            int before = Skills.Level(player, def.Skill, rules.Skills);
            Skills.AddXp(player, def.Skill, o.XpGained);
            o.NewLevel = Skills.Level(player, def.Skill, rules.Skills);
            o.LeveledUp = o.NewLevel > before;

            return o;
        }
    }

    public static class Skills
    {
        public static int Xp (PlayerState p, string skill) => p.SkillXp.TryGetValue(skill, out var xp) ? xp : 0;

        public static void AddXp (PlayerState p, string skill, int amount)
        {
            if (amount <= 0) return;
            p.SkillXp[skill] = Xp(p, skill) + amount;
        }

        public static int Level (PlayerState p, string skill, SkillRules rules)
        {
            int xp = Xp(p, skill);
            int level = 0;
            foreach (var t in rules.LevelThresholds)
            {
                if (xp >= t) level++;
                else break;
            }
            return level;
        }

        // Modifier equals level: +0 at start, up to +5 with the default thresholds.
        public static int Modifier (PlayerState p, string skill, SkillRules rules) => Level(p, skill, rules);
    }
}
