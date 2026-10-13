using AnotherLife.Core.Rules;
using Xunit;

namespace AnotherLife.Core.Tests
{
    public class CheckTests
    {
        readonly Data.GameData data = TestData.Shipped();

        [Fact]
        public void DcScalesWithVehicleClassAndValue ()
        {
            var def = data.Checks.Action("vehicle.entry.lockpick");
            // Super car (+6) worth $60k (+2) on a base-12 lock.
            int dc = Checks.ComputeDc(def, data.Checks, new CheckContext { VehicleClass = 7, VehicleValue = 60000 });
            Assert.Equal(20, dc);
        }

        [Fact]
        public void NonVehicleActionsIgnoreVehicleClass ()
        {
            var def = data.Checks.Action("body.search");
            Assert.Equal(8, Checks.ComputeDc(def, data.Checks, new CheckContext { VehicleClass = 7 }));
        }

        [Fact]
        public void ToolAdvantageAndWantedDisadvantageCancel ()
        {
            var def = data.Checks.Action("vehicle.hotwire");
            Assert.Equal(RollMode.Advantage, Checks.ResolveMode(def, new CheckContext { HasTool = true }, false));
            Assert.Equal(RollMode.Disadvantage, Checks.ResolveMode(def, new CheckContext { Wanted = true }, false));
            Assert.Equal(RollMode.Normal, Checks.ResolveMode(def, new CheckContext { HasTool = true, Wanted = true }, false));
        }

        [Fact]
        public void AdvantageTakesTheHigherDie ()
        {
            var p = TestData.NewPlayer(data);
            var o = Checks.Attempt("vehicle.hotwire", data, p, new CheckContext { HasTool = true }, new ScriptedRandom(4, 17));
            Assert.Equal(RollMode.Advantage, o.Mode);
            Assert.Equal(17, o.Natural);
            Assert.True(o.Success);
        }

        [Fact]
        public void NaturalOneFailsBreaksToolAndAddsStars ()
        {
            var p = TestData.NewPlayer(data);
            var o = Checks.Attempt("vehicle.entry.lockpick", data, p, new CheckContext { HasTool = true, DcAdjust = -20 }, new ScriptedRandom(1, 1));
            Assert.False(o.Success);
            Assert.True(o.ToolBroke);
            Assert.Equal(1, o.WantedStarsAdded);
        }

        [Fact]
        public void NaturalTwentySucceedsAndGrantsMomentumForTheNextCheck ()
        {
            var p = TestData.NewPlayer(data);
            var first = Checks.Attempt("vehicle.entry.smash", data, p, new CheckContext { DcAdjust = 30 }, new ScriptedRandom(20));
            Assert.True(first.Success);
            Assert.True(first.MomentumGained);

            var second = Checks.Attempt("body.search", data, p, new CheckContext(), new ScriptedRandom(3, 9));
            Assert.True(second.MomentumUsed);
            Assert.Equal(RollMode.Advantage, second.Mode);
            Assert.False(p.Momentum);
        }

        [Fact]
        public void PracticeLevelsSkillAndRaisesModifier ()
        {
            var p = TestData.NewPlayer(data);
            Assert.Equal(0, Skills.Modifier(p, "brute", data.Rules.Skills));

            // Base DC 11 success = 11 XP; 6 successes = 66 XP >= 60 threshold.
            CheckOutcome last = null;
            for (int i = 0; i < 6; i++)
                last = Checks.Attempt("vehicle.entry.smash", data, p, new CheckContext(), new ScriptedRandom(15));

            Assert.True(last.LeveledUp);
            Assert.Equal(1, Skills.Modifier(p, "brute", data.Rules.Skills));
            var next = Checks.Attempt("vehicle.entry.smash", data, p, new CheckContext(), new ScriptedRandom(10));
            Assert.Equal(11, next.Total);
        }

        [Fact]
        public void FailedAttemptsStillEarnSomePractice ()
        {
            var p = TestData.NewPlayer(data);
            var o = Checks.Attempt("vehicle.hotwire", data, p, new CheckContext(), new ScriptedRandom(2));
            Assert.False(o.Success);
            Assert.Equal(5, o.XpGained); // 14 * 0.34 rounded
        }

        [Fact]
        public void MissingToolIsRejectedWithoutRolling ()
        {
            var p = TestData.NewPlayer(data);
            var o = Checks.Attempt("hack.panel", data, p, new CheckContext(), new ScriptedRandom());
            Assert.Equal("tool_required", o.Error);
        }
    }
}

namespace AnotherLife.Core.Tests
{
    public class ToolTests
    {
        readonly Data.GameData data = TestData.Shipped();

        [Fact]
        public void CarriedToolGivesAdvantageAndBreaksOnANaturalOne ()
        {
            var p = TestData.NewPlayer(data);
            Inventory.Add(p, "screwdriver");
            var def = data.Checks.Action("vehicle.hotwire");
            var ctx = Checks.ForPlayer(def, p, new CheckContext());
            Assert.Equal("screwdriver", ctx.ToolItem);

            var o = Checks.Attempt("vehicle.hotwire", data, p, ctx, new ScriptedRandom(1, 1));
            Assert.Equal(RollMode.Advantage, o.Mode);
            Assert.Equal("screwdriver", o.ToolItem);
            Assert.False(Inventory.Has(p, "screwdriver"));
        }

        [Fact]
        public void BestListedToolIsUsedFirst ()
        {
            var p = TestData.NewPlayer(data);
            Inventory.Add(p, "lockpick");
            Inventory.Add(p, "slim_jim");
            var ctx = Checks.ForPlayer(data.Checks.Action("vehicle.entry.lockpick"), p, new CheckContext());
            Assert.Equal("slim_jim", ctx.ToolItem);
        }
    }
}
