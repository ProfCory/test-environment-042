using System;
using AnotherLife.Core.Mail;
using GTA;
using GTA.UI;
using LemonUI.Menus;

namespace AnotherLife.InGame
{
    // F10 > Mail. Delivers .txt files dropped into scripts/AnotherLife/mail/inbox while you play,
    // pings you with a text, and lets you reread or delete them.
    internal sealed class MailMenu
    {
        const int PollMs = 5000;
        const int RowWidth = 44;   // characters that fit one LemonUI menu row

        public readonly NativeMenu Inbox = new NativeMenu("Mail", "Inbox");
        public readonly NativeMenu Reader = new NativeMenu("Mail", "");

        readonly Main main;
        Mailbox box;
        int sincePoll = PollMs;

        public MailMenu (Main main)
        {
            this.main = main;
            box = Mailbox.Load(main.Paths.Mailbox, out var error);
            if (error != null)
            {
                Log.Write("mail: " + error);
                Notification.Show("~r~Mailbox was corrupt~s~, a copy was kept.");
            }
            Inbox.MaxItems = 12;
            Reader.MaxItems = 14;
            Reader.Parent = Inbox;
            Inbox.Shown += (s, e) => RebuildInbox();
        }

        public string Title => box.Unread > 0 ? $"Mail (~y~{box.Unread} unread~s~)" : "Mail";

        public void Tick (int deltaMs)
        {
            sincePoll += deltaMs;
            if (sincePoll < PollMs || Game.IsLoading) return;
            sincePoll = 0;

            try
            {
                var arrived = box.Collect(main.Paths.MailInbox, DateTime.Now);
                if (arrived.Count == 0) return;
                box.Save(main.Paths.Mailbox);
                foreach (var m in arrived)
                {
                    Notification.Show(NotificationIcon.Default, m.From, "New email", $"{m.Subject} ~c~(F10 > Mail)");
                    Log.Write($"mail: delivered \"{m.Subject}\" from {m.From}");
                }
                if (Inbox.Visible) RebuildInbox();
            }
            catch (Exception ex)
            {
                Log.Write("mail: " + ex);
            }
        }

        void RebuildInbox ()
        {
            Inbox.Clear();
            if (box.Messages.Count == 0)
            {
                Inbox.Add(new NativeItem("No mail yet", "Drop a .txt into scripts/AnotherLife/mail/inbox."));
                return;
            }
            foreach (var m in box.Messages)
            {
                var msg = m;
                var item = new NativeItem((msg.Read ? "" : "~y~") + msg.Subject, $"From {msg.From}, {msg.Received:MMM d HH:mm}");
                item.Activated += (s, e) => Open(msg);
                Inbox.Add(item);
            }
        }

        void Open (MailMessage m)
        {
            if (!m.Read)
            {
                m.Read = true;
                box.Save(main.Paths.Mailbox);
            }

            Reader.Clear();
            Reader.Name = m.Subject;
            Reader.Add(new NativeItem("~c~From: " + m.From, $"Received {m.Received:MMM d, HH:mm}"));
            foreach (var row in Mailbox.Wrap(m.Body, RowWidth)) Reader.Add(new NativeItem(row.Length == 0 ? " " : row));

            var delete = new NativeItem("~r~Delete this email");
            delete.Activated += (s, e) =>
            {
                box.Messages.Remove(m);
                box.Save(main.Paths.Mailbox);
                Reader.Back();
            };
            Reader.Add(delete);

            Inbox.Visible = false;
            Reader.Visible = true;
        }
    }
}
