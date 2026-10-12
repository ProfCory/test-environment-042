using System;
using System.Collections.Generic;
using System.IO;
using AnotherLife.Core;
using AnotherLife.Core.Data;
using AnotherLife.Core.Rules;

namespace AnotherLife.Core.Tests
{
    // Hands out scripted dice so every outcome in a test is deliberate.
    sealed class ScriptedRandom : IRandom
    {
        readonly Queue<int> rolls;
        public ScriptedRandom (params int[] rolls) { this.rolls = new Queue<int>(rolls); }
        public int Next (int minInclusive, int maxInclusive) => rolls.Dequeue();
        public double NextDouble () => 0.5;
    }

    static class TestData
    {
        public static string DataDir ()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null && !File.Exists(Path.Combine(dir.FullName, "data", "rules.json"))) dir = dir.Parent;
            if (dir == null) throw new InvalidOperationException("repo data folder not found");
            return Path.Combine(dir.FullName, "data");
        }

        // The shipped data, so tests also prove the JSON players get is valid.
        public static GameData Shipped ()
        {
            var result = DataLoader.Load(DataDir());
            if (!result.Ok) throw new InvalidOperationException(string.Join("; ", result.Errors));
            return result.Data;
        }

        public static PlayerState NewPlayer (GameData data) => SaveStore.NewGame(data);
    }
}
