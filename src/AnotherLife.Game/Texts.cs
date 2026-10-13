using System.Linq;
using AnotherLife.Core.Rules;
using GTA.UI;

namespace AnotherLife.InGame
{
    // Player-facing messages. Plain notifications for M0; phone texts replace these in M2.
    internal static class Texts
    {
        // Controls cheat sheet as phone-style texts. They stay rereadable in Pause > Brief > Notifications.
        public static void Controls ()
        {
            const string from = "Another Life";
            Notification.Show(NotificationIcon.Default, from, "Controls 1/4", "~b~F10~s~ Another Life menu. ~b~F9~s~ reload data. ~b~E~s~ at ATMs and payphones.");
            Notification.Show(NotificationIcon.Default, from, "Controls 2/4", "~b~F8~s~ Menyoo. ~b~F7~s~ Spooner. ~b~F3~s~ free cam. ~b~Shift+X~s~ Freemode Identity.");
            Notification.Show(NotificationIcon.Default, from, "Controls 3/4", "~b~F4~s~ SHVDN console. ~b~Backspace~s~ backs out of menus.");
            Notification.Show(NotificationIcon.Default, from, "Controls 4/4", "Reread: Pause > Brief > Notifications. Your emails: F10 > Mail.");
        }

        public static void Death (DeathResult r)
        {
            switch (r.Bill)
            {
                case BillPayment.Cash: Notification.Show($"~r~Hospital~s~: ${r.BillAmount} paid in cash."); break;
                case BillPayment.Bank: Notification.Show($"~r~Hospital~s~: ${r.BillAmount} charged to your card."); break;
                case BillPayment.PartialCash: Notification.Show("~r~Hospital~s~ took what cash you had."); break;
                default: Notification.Show("~r~Hospital~s~: no money, no card. They let it go."); break;
            }
            if (r.CashLost > 0 || r.CryptoLost > 0 || r.ItemsLost.Count > 0)
                Notification.Show($"Lost: ${r.CashLost}, {r.CryptoLost:0.####} crypto, {r.ItemsLost.Sum(s => s.Count)} items.");
        }

        public static void Arrest (ArrestResult r)
        {
            Notification.Show($"~b~Booked~s~. Seized ${r.CashSeized}, {r.CryptoSeized:0.####} crypto, {r.ItemsSeized.Sum(s => s.Count)} items.");
            if (r.FakeIdFlagged) Notification.Show("~o~Your fake ID is flagged.~s~ Get a new one or every cop knows you.");
            if (r.Fee == BillPayment.Bank) Notification.Show($"${r.FeeAmount} booking fee charged to your card.");
            if (r.BondTaken) Notification.Show($"~y~Bail bondsman~s~ covered ${r.FeeAmount}. His guys will collect.");
            if (r.NoBail) Notification.Show("~r~No bail.~s~ The bondsman won't front you more. You did the time.");
            if (r.Impounded != null) Notification.Show($"Car {r.Impounded.Plate} impounded. ${r.Impounded.Fee} to get it back.");
            if (r.PermanentStar) Notification.Show("~r~Every cop in the county knows your face now.");
        }

        public static void Check (CheckOutcome o, bool showDc)
        {
            if (o.Error != null)
            {
                Notification.Show($"~r~{o.ActionId}~s~: {o.Error}");
                return;
            }
            string dice = o.Mode == RollMode.Normal ? $"{o.Natural}" : $"{o.RollA}/{o.RollB} {(o.Mode == RollMode.Advantage ? "adv" : "dis")}";
            string dc = showDc ? $" vs DC {o.Dc}" : "";
            string verdict = o.Success ? "~g~SUCCESS~s~" : "~r~FAIL~s~";
            Notification.Show($"{o.Label}: d20 {dice} {o.Modifier:+0;-0} = {o.Total}{dc}  {verdict}");
            if (o.Nat20) Notification.Show("~g~Natural 20!~s~ Advantage on your next check.");
            if (o.Nat1) Notification.Show("~r~Natural 1.~s~" + (o.ToolItem != null ? $" Your {o.ToolItem} broke." : o.ToolBroke ? " Tool broke." : "") + (o.WantedStarsAdded > 0 ? $" +{o.WantedStarsAdded} stars." : ""));
            if (o.LeveledUp) Notification.Show($"~b~{o.Skill}~s~ improved to +{o.NewLevel}.");
        }
    }
}
