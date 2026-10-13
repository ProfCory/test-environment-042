using System;
using System.IO;
using AnotherLife.Core.Data;
using AnotherLife.Core.Rules;
using Newtonsoft.Json;

namespace AnotherLife.Core
{
    public static class SaveStore
    {
        static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };

        public static PlayerState NewGame (GameData data)
        {
            var p = new PlayerState
            {
                Cash = data.Rules.Start.Cash,
                CryptoRate = data.Rules.Economy.CryptoStartRate
            };
            foreach (var grant in data.Rules.Start.Items) Inventory.Add(p, grant.Item, grant.Count);
            return p;
        }

        // Returns null when there is no save yet. A corrupt save is moved aside, not deleted.
        public static PlayerState Load (string path, out string error)
        {
            error = null;
            if (!File.Exists(path)) return null;
            try
            {
                return JsonConvert.DeserializeObject<PlayerState>(File.ReadAllText(path), Settings);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                File.Copy(path, path + ".corrupt-" + DateTime.Now.ToString("yyyyMMdd-HHmmss"), true);
                return null;
            }
        }

        // Write to a temp file then swap, keeping the previous save as .bak.
        public static void Save (string path, PlayerState state)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var tmp = path + ".tmp";
            File.WriteAllText(tmp, JsonConvert.SerializeObject(state, Settings));
            if (File.Exists(path)) File.Copy(path, path + ".bak", true);
            File.Copy(tmp, path, true);
            File.Delete(tmp);
        }
    }
}
