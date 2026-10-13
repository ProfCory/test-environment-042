using System.Collections.Generic;
using AnotherLife.Core.Data;

namespace AnotherLife.Core.Rules
{
    // Every spot on the map has an address: nearest postal + the game's street and zone names.
    public static class Addresses
    {
        public static PostalDef NearestPostal (List<PostalDef> postals, float x, float y)
        {
            PostalDef best = null;
            float bestDist = float.MaxValue;
            foreach (var p in postals)
            {
                float dx = p.X - x, dy = p.Y - y;
                float d = dx * dx + dy * dy;
                if (d < bestDist) { bestDist = d; best = p; }
            }
            return best;
        }

        public static PostalDef FindPostal (List<PostalDef> postals, string code)
        {
            code = code?.Trim().TrimStart('#');
            return postals.Find(p => p.Code == code);
        }

        // "#4021 Elgin Ave, Downtown"; any missing part is dropped.
        public static string Format (string postalCode, string street, string zone)
        {
            var parts = new List<string>();
            string line = string.IsNullOrEmpty(postalCode) ? street : string.IsNullOrEmpty(street) ? "#" + postalCode : $"#{postalCode} {street}";
            if (!string.IsNullOrEmpty(line)) parts.Add(line);
            if (!string.IsNullOrEmpty(zone)) parts.Add(zone);
            return string.Join(", ", parts);
        }
    }
}
