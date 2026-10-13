using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace AnotherLife.Core.Mail
{
    public sealed class MailMessage
    {
        public string From;
        public string Subject;
        public string Body;
        public DateTime Received;
        public bool Read;
    }

    // The in-game inbox. Kept apart from the game save so notes survive "Start a new game".
    // Mail arrives two ways: the mod sends it (Send), or a .txt file is dropped into the inbox
    // folder (Collect). File format: optional "From:" / "Subject:" header lines, a blank line, the body.
    public sealed class Mailbox
    {
        public const string DefaultFrom = "Claude";

        static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };

        // Newest first.
        public List<MailMessage> Messages = new List<MailMessage>();

        [JsonIgnore] public int Unread => Messages.Count(m => !m.Read);

        // A missing mailbox is empty; a corrupt one is moved aside, not deleted.
        public static Mailbox Load (string path, out string error)
        {
            error = null;
            if (!File.Exists(path)) return new Mailbox();
            try
            {
                return JsonConvert.DeserializeObject<Mailbox>(File.ReadAllText(path), Settings) ?? new Mailbox();
            }
            catch (Exception ex)
            {
                error = ex.Message;
                File.Copy(path, path + ".corrupt-" + DateTime.Now.ToString("yyyyMMdd-HHmmss"), true);
                return new Mailbox();
            }
        }

        public void Save (string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var tmp = path + ".tmp";
            File.WriteAllText(tmp, JsonConvert.SerializeObject(this, Settings));
            File.Copy(tmp, path, true);
            File.Delete(tmp);
        }

        public MailMessage Send (string from, string subject, string body, DateTime now)
        {
            var m = new MailMessage { From = from, Subject = subject, Body = body ?? "", Received = now };
            Messages.Insert(0, m);
            return m;
        }

        // Delivers every *.txt in inboxDir (oldest name first) and moves it to inboxDir/delivered.
        // A file that can't be read yet (still being written) is left for the next pass.
        public List<MailMessage> Collect (string inboxDir, DateTime now)
        {
            var delivered = new List<MailMessage>();
            if (!Directory.Exists(inboxDir)) return delivered;
            var done = Path.Combine(inboxDir, "delivered");

            foreach (var file in Directory.GetFiles(inboxDir, "*.txt").OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
            {
                string text;
                try { text = File.ReadAllText(file, Encoding.UTF8); }
                catch (IOException) { continue; }

                var m = Parse(text, Path.GetFileNameWithoutExtension(file));
                m.Received = now;
                Messages.Insert(0, m);
                delivered.Add(m);

                Directory.CreateDirectory(done);
                var target = Path.Combine(done, Path.GetFileName(file));
                if (File.Exists(target)) target = Path.Combine(done, $"{Path.GetFileNameWithoutExtension(file)}-{now:yyyyMMdd-HHmmss}.txt");
                File.Move(file, target);
            }
            return delivered;
        }

        public static MailMessage Parse (string text, string fallbackSubject)
        {
            var m = new MailMessage { From = DefaultFrom, Subject = fallbackSubject };
            var lines = (text ?? "").Replace("\r\n", "\n").Split('\n');
            int i = 0;
            for (; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.Trim().Length == 0) { i++; break; }
                int colon = line.IndexOf(':');
                string key = colon > 0 ? line.Substring(0, colon).Trim().ToLowerInvariant() : null;
                if (key == "from") m.From = line.Substring(colon + 1).Trim();
                else if (key == "subject") m.Subject = line.Substring(colon + 1).Trim();
                else break; // no header block: the whole file is body
            }
            m.Body = string.Join("\n", lines.Skip(i)).Trim('\n');
            return m;
        }

        // Splits a body into display rows of at most `width` characters. Blank lines are kept
        // as paragraph breaks; a word longer than a row is cut.
        public static List<string> Wrap (string body, int width)
        {
            var rows = new List<string>();
            foreach (var paragraph in (body ?? "").Replace("\r\n", "\n").Split('\n'))
            {
                var row = new StringBuilder();
                foreach (var word in paragraph.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var w = word;
                    if (row.Length > 0 && row.Length + 1 + w.Length > width) { rows.Add(row.ToString()); row.Clear(); }
                    while (w.Length > width) { rows.Add(w.Substring(0, width)); w = w.Substring(width); }
                    if (row.Length > 0) row.Append(' ');
                    row.Append(w);
                }
                rows.Add(row.ToString());
            }
            return rows;
        }
    }
}
