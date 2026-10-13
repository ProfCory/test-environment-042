using AnotherLife.Core.Rules;
using Xunit;

namespace AnotherLife.Core.Tests
{
    public class AddressTests
    {
        readonly Data.GameData data = TestData.Shipped();

        [Fact]
        public void ShipsTheFullNewPostalsSet ()
        {
            Assert.Equal(1687, data.Postals.Count);
        }

        [Fact]
        public void NearestPostalIsTheClosestPoint ()
        {
            var first = data.Postals[0];
            var nearest = Addresses.NearestPostal(data.Postals, first.X + 1, first.Y - 1);
            Assert.Equal(first.Code, nearest.Code);
        }

        [Fact]
        public void FindAcceptsAHashPrefixAndSpaces ()
        {
            Assert.Equal("2000", Addresses.FindPostal(data.Postals, " #2000 ").Code);
            Assert.Null(Addresses.FindPostal(data.Postals, "99999"));
        }

        [Theory]
        [InlineData("4021", "Elgin Ave", "Downtown", "#4021 Elgin Ave, Downtown")]
        [InlineData("4021", "", "Downtown", "#4021, Downtown")]
        [InlineData(null, "Elgin Ave", "Downtown", "Elgin Ave, Downtown")]
        [InlineData("4021", "Elgin Ave", null, "#4021 Elgin Ave")]
        public void FormatsAddresses (string postal, string street, string zone, string expected)
        {
            Assert.Equal(expected, Addresses.Format(postal, street, zone));
        }
    }
}
