using System.IO;
using AnotherLife.Core.Rules;
using Xunit;

namespace AnotherLife.Core.Tests
{
    public class SaveTests
    {
        readonly Data.GameData data = TestData.Shipped();

        [Fact]
        public void NewGameStartsWithFortyDollarsAFakeIdAndAPhone ()
        {
            var p = SaveStore.NewGame(data);
            Assert.Equal(40, p.Cash);
            Assert.False(p.HasBankAccount);
            Assert.True(Inventory.Has(p, Inventory.FakeId));
            Assert.True(Inventory.Has(p, "phone_burner"));
            Assert.Equal(250, p.CryptoRate);
        }

        [Fact]
        public void SaveRoundTripsAndKeepsABackup ()
        {
            var path = Path.Combine(Directory.CreateTempSubdirectory().FullName, "save", "slot1.json");
            var p = SaveStore.NewGame(data);
            p.SkillXp["brute"] = 77;
            SaveStore.Save(path, p);
            p.Cash = 999;
            SaveStore.Save(path, p);

            var loaded = SaveStore.Load(path, out var error);
            Assert.Null(error);
            Assert.Equal(999, loaded.Cash);
            Assert.Equal(77, loaded.SkillXp["brute"]);
            Assert.Equal(40, SaveStore.Load(path + ".bak", out _).Cash);
        }

        [Fact]
        public void CorruptSaveIsMovedAsideAndReported ()
        {
            var dir = Directory.CreateTempSubdirectory().FullName;
            var path = Path.Combine(dir, "slot1.json");
            File.WriteAllText(path, "{ not json");
            Assert.Null(SaveStore.Load(path, out var error));
            Assert.NotNull(error);
            Assert.Single(Directory.GetFiles(dir, "slot1.json.corrupt-*"));
        }
    }
}
