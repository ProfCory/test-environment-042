using System.Linq;
using AnotherLife.Core.Rules;
using Xunit;

namespace AnotherLife.Core.Tests
{
    public class PenaltyTests
    {
        readonly Data.GameData data = TestData.Shipped();

        // Downtown LS, so the nearest hospital/station is in the city.
        const float X = 200, Y = -900, Z = 30;

        PlayerState Loaded ()
        {
            var p = TestData.NewPlayer(data);
            Inventory.Add(p, "lockpick", 2);
            Inventory.Add(p, "burger");
            Inventory.Add(p, "key_vehicle", 1, "ALN 042");
            p.Crypto = 0.5;
            return p;
        }

        [Fact]
        public void DeathPaysBillFromCashThenLosesTheRest ()
        {
            var p = Loaded();
            p.Cash = 400;
            var r = Penalties.ApplyDeath(p, data, X, Y, Z);

            Assert.Equal(BillPayment.Cash, r.Bill);
            Assert.Equal(150, r.CashLost);
            Assert.Equal(0, p.Cash);
            Assert.Equal(0.5, r.CryptoLost);
            Assert.Equal(0, p.Crypto);
        }

        [Fact]
        public void DeathKeepsCardIdPhoneAndKeysOnly ()
        {
            var p = Loaded();
            Inventory.Add(p, Inventory.BankCard);
            Penalties.ApplyDeath(p, data, X, Y, Z);

            var left = p.Inventory.Select(s => s.Item).OrderBy(s => s).ToArray();
            Assert.Equal(new[] { "bank_card", "id_fake", "key_vehicle", "phone_burner" }, left);
        }

        [Fact]
        public void DeathBillGoesOnTheCardWhenCashIsShort ()
        {
            var p = Loaded();
            p.Cash = 100;
            p.HasBankAccount = true;
            Inventory.Add(p, Inventory.BankCard);
            var r = Penalties.ApplyDeath(p, data, X, Y, Z);

            Assert.Equal(BillPayment.Bank, r.Bill);
            Assert.Equal(-250, p.Bank);
            Assert.Equal(100, r.CashLost);
        }

        [Fact]
        public void DeathWithNoCardTakesWhatCashThereIs ()
        {
            var p = Loaded();
            p.Cash = 30;
            var r = Penalties.ApplyDeath(p, data, X, Y, Z);
            Assert.Equal(BillPayment.PartialCash, r.Bill);
            Assert.Equal(0, r.CashLost);
        }

        [Fact]
        public void DeathRespawnsAtNearestHospital ()
        {
            var r = Penalties.ApplyDeath(Loaded(), data, -250, 6300, 32);
            Assert.Equal("svc_hospital_paleto", r.Respawn.Id);
        }

        [Fact]
        public void ArrestSeizesEverythingAndFlagsTheFakeId ()
        {
            var p = Loaded();
            p.Cash = 400;
            var r = Penalties.ApplyArrest(p, data, X, Y, Z, null);

            Assert.Equal(400, r.CashSeized);
            Assert.True(r.FakeIdFlagged);
            Assert.True(p.FakeIdFlagged);
            Assert.True(Inventory.Has(p, Inventory.FakeId));
            Assert.False(Inventory.Has(p, "lockpick"));
            Assert.Equal("svc_police_missionrow", r.Respawn.Id);
        }

        [Fact]
        public void ArrestFeeGoesToBankWhenPossible ()
        {
            var p = Loaded();
            p.HasBankAccount = true;
            Inventory.Add(p, Inventory.BankCard);
            var r = Penalties.ApplyArrest(p, data, X, Y, Z, null);
            Assert.Equal(BillPayment.Bank, r.Fee);
            Assert.Equal(-100, p.Bank);
            Assert.Equal(0, p.BondDebt);
        }

        [Fact]
        public void NoBankMeansBailBondUpToTheCapThenNoBail ()
        {
            var p = Loaded();
            p.BondDebt = 900;
            var r = Penalties.ApplyArrest(p, data, X, Y, Z, null);
            Assert.True(r.BondTaken);
            Assert.Equal(1000, p.BondDebt);

            var again = Penalties.ApplyArrest(p, data, X, Y, Z, null);
            Assert.True(again.NoBail);
            Assert.Equal(1000, p.BondDebt);
        }

        [Fact]
        public void DrivingYourOwnCarGetsItImpounded ()
        {
            var p = Loaded();
            var r = Penalties.ApplyArrest(p, data, X, Y, Z, "ALN 042");
            Assert.Equal(500, r.Impounded.Fee);
            Assert.Single(p.Impounds);
            Assert.Equal("svc_police_missionrow", p.Impounds[0].LocationId);
        }

        [Fact]
        public void FlaggedIdPlusArrestsLeadToAPermanentStar ()
        {
            var p = Loaded();
            p.CopRep = -20;
            var r = Penalties.ApplyArrest(p, data, X, Y, Z, null);
            // -20 -10 arrest -25 flagged = -55: not yet.
            Assert.False(r.PermanentStar);
            var r2 = Penalties.ApplyArrest(p, data, X, Y, Z, null);
            Assert.True(r2.PermanentStar);

            p.FakeIdFlagged = false; // bought a clean fake
            Assert.False(Penalties.HasPermanentStar(p, data.Rules.Arrest));
        }
    }
}
