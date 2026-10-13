using System;
using System.IO;
using AnotherLife.Core.Mail;
using Xunit;

namespace AnotherLife.Core.Tests
{
    public class MailTests
    {
        static readonly DateTime Now = new DateTime(2026, 10, 12, 18, 0, 0);

        [Fact]
        public void ParseReadsHeadersThenBody ()
        {
            var m = Mailbox.Parse("From: Claude\r\nSubject: Test the ATM\r\n\r\nWithdraw $20.\r\nCheck the fee.", "file");
            Assert.Equal("Claude", m.From);
            Assert.Equal("Test the ATM", m.Subject);
            Assert.Equal("Withdraw $20.\nCheck the fee.", m.Body);
        }

        [Fact]
        public void FileWithoutHeadersIsAllBodyAndNamedAfterTheFile ()
        {
            var m = Mailbox.Parse("Remember: buy a lockpick.\nThen hotwire.", "002-lockpick");
            Assert.Equal(Mailbox.DefaultFrom, m.From);
            Assert.Equal("002-lockpick", m.Subject);
            Assert.Equal("Remember: buy a lockpick.\nThen hotwire.", m.Body);
        }

        [Fact]
        public void CollectDeliversNewestFirstAndMovesFilesAside ()
        {
            var inbox = Directory.CreateTempSubdirectory().FullName;
            File.WriteAllText(Path.Combine(inbox, "001.txt"), "Subject: first\n\none");
            File.WriteAllText(Path.Combine(inbox, "002.txt"), "Subject: second\n\ntwo");
            File.WriteAllText(Path.Combine(inbox, "notes.md"), "ignored");

            var box = new Mailbox();
            var got = box.Collect(inbox, Now);

            Assert.Equal(2, got.Count);
            Assert.Equal("second", box.Messages[0].Subject);
            Assert.Equal(2, box.Unread);
            Assert.Empty(Directory.GetFiles(inbox, "*.txt"));
            Assert.Equal(2, Directory.GetFiles(Path.Combine(inbox, "delivered")).Length);
            Assert.Empty(box.Collect(inbox, Now));
        }

        [Fact]
        public void ResendingTheSameFileNameKeepsBothCopies ()
        {
            var inbox = Directory.CreateTempSubdirectory().FullName;
            var box = new Mailbox();
            File.WriteAllText(Path.Combine(inbox, "a.txt"), "one");
            box.Collect(inbox, Now);
            File.WriteAllText(Path.Combine(inbox, "a.txt"), "two");
            box.Collect(inbox, Now.AddMinutes(1));
            Assert.Equal(2, Directory.GetFiles(Path.Combine(inbox, "delivered")).Length);
        }

        [Fact]
        public void MailboxRoundTripsAndSurvivesCorruption ()
        {
            var path = Path.Combine(Directory.CreateTempSubdirectory().FullName, "mail", "mailbox.json");
            var box = new Mailbox();
            box.Send("Another Life", "Hi", "Body", Now).Read = true;
            box.Save(path);

            var loaded = Mailbox.Load(path, out var error);
            Assert.Null(error);
            Assert.Single(loaded.Messages);
            Assert.True(loaded.Messages[0].Read);

            File.WriteAllText(path, "{ nope");
            Assert.Empty(Mailbox.Load(path, out error).Messages);
            Assert.NotNull(error);
        }

        [Fact]
        public void WrapKeepsParagraphsAndFitsTheWidth ()
        {
            var rows = Mailbox.Wrap("one two three four\n\nsupercalifragilistic", 10);
            Assert.Equal(new[] { "one two", "three four", "", "supercalif", "ragilistic" }, rows);
        }
    }
}
