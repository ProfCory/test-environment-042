using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace AnotherLife.Core.Data
{
    public sealed class LoadResult
    {
        public GameData Data;
        public List<string> Errors = new List<string>();
        public bool Ok => Errors.Count == 0;
    }

    // Loads every data file, collecting problems instead of throwing, so a typo in one file
    // shows up as an in-game message rather than killing the script.
    public static class DataLoader
    {
        public const string Rules = "rules.json";
        public const string Checks = "checks.json";
        public const string Items = "items.json";
        public const string Locations = "locations.json";
        public const string CrashPads = "crashpads.json";

        static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Error,
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };

        public static LoadResult Load (string dataDir)
        {
            var result = new LoadResult { Data = new GameData() };
            var data = result.Data;

            data.Rules = Read<RulesConfig>(dataDir, Rules, result.Errors) ?? new RulesConfig();
            data.Checks = Read<ChecksData>(dataDir, Checks, result.Errors) ?? new ChecksData();
            data.Locations = Read<List<LocationDef>>(dataDir, Locations, result.Errors) ?? new List<LocationDef>();
            data.CrashPads = Read<List<CrashPadDef>>(dataDir, CrashPads, result.Errors) ?? new List<CrashPadDef>();

            var items = Read<List<ItemDef>>(dataDir, Items, result.Errors) ?? new List<ItemDef>();
            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item.Id)) { result.Errors.Add($"{Items}: item with no id"); continue; }
                if (data.Items.ContainsKey(item.Id)) { result.Errors.Add($"{Items}: duplicate id '{item.Id}'"); continue; }
                data.Items[item.Id] = item;
            }

            Validate(data, result.Errors);
            return result;
        }

        static T Read<T> (string dir, string file, List<string> errors) where T : class
        {
            var path = Path.Combine(dir, file);
            if (!File.Exists(path))
            {
                errors.Add($"{file}: missing");
                return null;
            }
            try
            {
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(path), Settings);
            }
            catch (Exception ex)
            {
                errors.Add($"{file}: {ex.Message}");
                return null;
            }
        }

        static void Validate (GameData data, List<string> errors)
        {
            var skills = new HashSet<string>(data.Rules.Skills.Skills);
            var actionIds = new HashSet<string>();
            foreach (var a in data.Checks.Actions)
            {
                if (!actionIds.Add(a.Id)) errors.Add($"{Checks}: duplicate action '{a.Id}'");
                if (!skills.Contains(a.Skill)) errors.Add($"{Checks}: action '{a.Id}' uses unknown skill '{a.Skill}'");
                foreach (var tool in a.Tools)
                {
                    if (data.Item(tool) == null) errors.Add($"{Checks}: action '{a.Id}' tool '{tool}' not in {Items}");
                }
                if ((a.RequiresTool || a.ToolGivesAdvantage) && a.Tools.Count == 0)
                    errors.Add($"{Checks}: action '{a.Id}' uses a tool but lists no tools");
            }

            var thresholds = data.Rules.Skills.LevelThresholds;
            for (int i = 1; i < thresholds.Count; i++)
            {
                if (thresholds[i] <= thresholds[i - 1]) errors.Add($"{Rules}: skills.levelThresholds must increase");
            }

            var locationIds = new HashSet<string>();
            foreach (var l in data.Locations)
            {
                if (!locationIds.Add(l.Id)) errors.Add($"{Locations}: duplicate id '{l.Id}'");
            }

            foreach (var grant in data.Rules.Start.Items)
            {
                if (data.Item(grant.Item) == null) errors.Add($"{Rules}: start item '{grant.Item}' not in {Items}");
            }

            if (data.Rules.Economy.BankOverdraftLimit > 0) errors.Add($"{Rules}: economy.bankOverdraftLimit must be 0 or negative");
        }
    }
}
