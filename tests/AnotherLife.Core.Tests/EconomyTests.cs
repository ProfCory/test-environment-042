using AnotherLife.Core.Rules;
using Xunit;

namespace AnotherLife.Core.Tests
{
    public class EconomyTests
    {
        readonly Data.GameData data = TestData.Shipped();

        PlayerState Banked (long balance)
        {
            var p = TestData.NewPlayer(data);
            p.HasBankAccount = true;
            p.Bank = balance;
            Inventory.Add(p, Inventory.BankCard);
            return p;
        }

        [Fact]
        public void CardRequiresAccountAndCard ()
        {
            var p = TestData.NewPlayer(data);
            p.HasBankAccount = true;
            p.Bank = 1000;
            Assert.False(Economy.TrySpend(p, 10, PayMethod.Card, data.Rules.Economy));
            Inventory.Add(p, Inventory.BankCard);
            Assert.True(Economy.TrySpend(p, 10, PayMethod.Card, data.Rules.Economy));
        }

        [Fact]
        public void BankCanGoNegativeUpToTheOverdraftLimit ()
        {
            var p = Banked(0);
            Assert.True(Economy.TrySpend(p, 5000, PayMethod.Card, data.Rules.Economy));
            Assert.Equal(-5000, p.Bank);
            Assert.False(Economy.TrySpend(p, 1, PayMethod.Card, data.Rules.Economy));
        }

        [Fact]
        public void CollectorsStartAtTheLimitAndStopOnceBackToZero ()
        {
            var p = Banked(0);
            Economy.TrySpend(p, 5000, PayMethod.Card, data.Rules.Economy);
            Assert.True(p.CollectorsActive);

            Economy.Deposit(p, 3000, data.Rules.Economy);
            Assert.True(p.CollectorsActive); // still -2000: they keep coming

            Economy.Deposit(p, 2000, data.Rules.Economy);
            Assert.False(p.CollectorsActive);
        }

        [Fact]
        public void ThugsTakeCarriedCashUntilBondIsPaid ()
        {
            var p = TestData.NewPlayer(data);
            p.BondDebt = 300;
            p.Cash = 120;
            Assert.Equal(120, Economy.ThugShakedown(p));
            Assert.Equal(180, p.BondDebt);
            p.Cash = 500;
            Assert.Equal(180, Economy.ThugShakedown(p));
            Assert.Equal(0, p.BondDebt);
            Assert.Equal(320, p.Cash);
        }

        [Fact]
        public void PhoneOrdersCostMarkupPlusDelivery ()
        {
            Assert.Equal(125, Economy.OrderPrice(100, data.Rules.Economy));
        }

        [Fact]
        public void CryptoRateStaysInsideItsBand ()
        {
            var p = TestData.NewPlayer(data);
            p.CryptoRate = 61;
            var down = new AlwaysLow();
            for (int i = 0; i < 50; i++) Economy.UpdateCryptoRate(p, data.Rules.Economy, down);
            Assert.Equal(60, p.CryptoRate);
        }

        [Fact]
        public void LaunderingConvertsAtRateMinusFee ()
        {
            var p = TestData.NewPlayer(data);
            p.Cash = 0;
            p.Crypto = 1.0;
            p.CryptoRate = 200;
            Assert.Equal(150, Economy.Launder(p, 1.0, 0.25));
            Assert.Equal(0, p.Crypto);
        }

        sealed class AlwaysLow : IRandom
        {
            public int Next (int a, int b) => a;
            public double NextDouble () => 0.0;
        }
    }
}
