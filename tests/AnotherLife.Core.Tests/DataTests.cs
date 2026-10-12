using System.IO;
using AnotherLife.Core.Data;
using Xunit;

namespace AnotherLife.Core.Tests
{
    public class DataTests
    {
        [Fact]
        public void ShippedDataLoadsWithoutErrors ()
        {
            var result = DataLoader.Load(TestData.DataDir());
            Assert.Empty(result.Errors);
            Assert.NotEmpty(result.Data.Checks.Actions);
            Assert.Equal(3, result.Data.CrashPads.Count);
            Assert.NotNull(result.Data.Item("bank_card"));
        }

        [Fact]
        public void MissingFileAndTyposAreReportedNotThrown ()
        {
            var dir = Directory.CreateTempSubdirectory().FullName;
            foreach (var f in Directory.GetFiles(TestData.DataDir())) File.Copy(f, Path.Combine(dir, Path.GetFileName(f)));
            File.Delete(Path.Combine(dir, DataLoader.CrashPads));
            File.WriteAllText(Path.Combine(dir, DataLoader.Rules), "{ \"startt\": {} }");

            var result = DataLoader.Load(dir);

            Assert.Contains(result.Errors, e => e.StartsWith("crashpads.json: missing"));
            Assert.Contains(result.Errors, e => e.StartsWith("rules.json:") && e.Contains("startt"));
            Assert.NotNull(result.Data.Rules);
        }

        [Fact]
        public void ActionWithUnknownSkillIsReported ()
        {
            var dir = Directory.CreateTempSubdirectory().FullName;
            foreach (var f in Directory.GetFiles(TestData.DataDir())) File.Copy(f, Path.Combine(dir, Path.GetFileName(f)));
            File.WriteAllText(Path.Combine(dir, DataLoader.Checks),
                "{ \"actions\": [ { \"id\": \"x\", \"label\": \"X\", \"skill\": \"juggling\", \"baseDc\": 10 } ] }");

            var result = DataLoader.Load(dir);

            Assert.Contains(result.Errors, e => e.Contains("unknown skill 'juggling'"));
        }
    }
}
