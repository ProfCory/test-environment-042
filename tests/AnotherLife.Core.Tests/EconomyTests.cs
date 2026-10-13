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
        public void AtmNeedsAccountAndCard ()
        {
            var p = TestData.NewPlayer(data);
            Assert.False(Economy.AtmDeposit(p, 20, data.Rules.Economy));
            Assert.Equal(40, p.Cash);
        }

        [Fact]
        public void AtmDepositIsFreeAndWithdrawCostsTheFee ()
        {
            var p = Banked(0);
            p.Cash = 100;
            Assert.True(Economy.AtmDeposit(p, 100, data.Rules.Economy));
            Assert.Equal(0, p.Cash);
            Assert.Equal(100, p.Bank);

            Assert.True(Economy.AtmWithdraw(p, 50, data.Rules.Economy));
            Assert.Equal(50, p.Cash);
            Assert.Equal(47, p.Bank);
        }

        [Fact]
        public void AtmWontPayOutOverdraftAsCash ()
        {
            var p = Banked(50);
            Assert.False(Economy.AtmWithdraw(p, 50, data.Rules.Economy)); // 50 + $3 fee > 50
            Assert.True(Economy.AtmWithdraw(p, 47, data.Rules.Economy));
            Assert.Equal(0, p.Bank);
        }

        [Fact]
        public void PayphoneWiresCryptoToBankMinusFee ()
        {
            var p = Banked(0);
            p.Crypto = 1.0;
            p.CryptoRate = 200;
            Assert.Equal(170, Economy.PayphoneWire(p, 1.0, 0.15, data.Rules.Economy));
            Assert.Equal(170, p.Bank);
            Assert.Equal(0, p.Crypto);
        }

        [Fact]
        public void PayphoneNeedsABankAccount ()
        {
            var p = TestData.NewPlayer(data);
            p.Crypto = 1.0;
            Assert.Equal(0, Economy.PayphoneWire(p, 1.0, 0.15, data.Rules.Economy));
            Assert.Equal(1.0, p.Crypto);
        }

        sealed class AlwaysLow : IRandom
        {
            public int Next (int a, int b) => a;
            public double NextDouble () => 0.0;
        }
    }
}
