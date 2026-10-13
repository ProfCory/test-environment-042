using System.Drawing;
using AnotherLife.Core.Rules;
using GTA;
using GTA.Math;
using GTA.UI;

namespace AnotherLife.InGame
{
    // "4021 | Elgin Ave, Downtown" beside the minimap, refreshed twice a second.
    internal sealed class AddressHud
    {
        const int RefreshMs = 500;

        // Just right of the minimap on a 1280x720 virtual screen (same spot nearest-postal uses).
        readonly TextElement text = new TextElement("", new PointF(288f, 690f), 0.4f, Color.White, GTA.UI.Font.ChaletLondon, Alignment.Left, true, true);
        readonly Main main;
        int sinceRefresh = RefreshMs;

        public string CurrentPostal { get; private set; }

        public AddressHud (Main main) { this.main = main; }

        public void Tick (int deltaMs)
        {
            if (!main.Data.Rules.Hud.ShowAddress || main.Data.Postals.Count == 0) return;

            sinceRefresh += deltaMs;
            if (sinceRefresh >= RefreshMs)
            {
                sinceRefresh = 0;
                var pos = Game.Player.Character.Position;
                CurrentPostal = Addresses.NearestPostal(main.Data.Postals, pos.X, pos.Y)?.Code;
                text.Caption = Describe(pos, CurrentPostal).Replace(", ", " | ").Replace("#", "");
            }

            if (!Game.IsPaused && Hud.IsRadarVisible) text.Draw();
        }

        public static string Describe (Vector3 at, string postal) =>
            Addresses.Format(postal, World.GetStreetName(at), World.GetZoneLocalizedName(at));

        public string AddressOf (Vector3 at) =>
            Describe(at, Addresses.NearestPostal(main.Data.Postals, at.X, at.Y)?.Code);

        // Sets the map waypoint to a postal code. Returns false if the code doesn't exist.
        public bool RouteTo (string code)
        {
            var postal = Addresses.FindPostal(main.Data.Postals, code);
            if (postal == null) return false;
            World.WaypointPosition = new Vector3(postal.X, postal.Y, 0f);
            return true;
        }
    }
}
