using BattleIntel.Core;
using GroupMe;
using GroupMe.Models;
using GSheet;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace BattleIntel.Bot
{
    class IntelBot
    {
        private readonly IIntelBotConsole console;
        private IntelBotSettings settings;
        private GroupMeService groupMe;
        private GSheetService google;

        public IntelBot(IIntelBotConsole console) 
        { 
            this.console = console;
            this.settings = new IntelBotSettings();
        }

        public IntelBotSettings GetSettings()
        {
            return settings.Clone();
        }

        public bool ConnectToBattle(IWin32Window owner)
        {
            using (var bs = new BattleSelector())
            {
                if (bs.ShowDialog(owner) == DialogResult.OK)
                {
                    settings.BattleId = bs.SelectedBattle.Id;
                    settings.BattleName = bs.SelectedBattle.Name;
                    settings.BattleStartDate = bs.SelectedBattle.StartDateUTC.ToLocalTime();
                    settings.BattleEndDate = bs.SelectedBattle.EndDateUTC.ToLocalTime();

                    console.AppendLine(string.Format("Connected to Battle: {0}/{1} ({2:D} - {3:D})", settings.BattleId, settings.BattleName, settings.BattleStartDate, settings.BattleEndDate));
                    return true;
                }
            }

            return false;
        }

        public bool ConnectToIntelRoom(IWin32Window owner)
        {
            if (groupMe == null)
            {
                groupMe = new GroupMeService(ConfigurationManager.AppSettings.Get("GroupMe-AccessToken"));
            }

            using (var gs = new GroupMeRoomSelector(groupMe))
            {
                if (gs.ShowDialog(owner) == DialogResult.OK)
                {
                    settings.GroupId = gs.SelectedGroup.id;
                    settings.GroupName = gs.SelectedGroup.name;
                    console.AppendLine(string.Format("Connected to GroupMeRoom: {0}/{1}", settings.GroupId, settings.GroupName));
                    return true;
                }
            }

            return false;
        }

        public bool ConnectToGoogleSheet(IWin32Window owner)
        {
            if (google == null)
            {
                var auth = GoogleOAuth.Authorize();
                if (auth == null) return false;
                google = new GSheetService(auth);
            }

            using (var ws = new WorksheetSelector(google))
            {
                if (ws.ShowDialog() == DialogResult.OK)
                {
                    settings.SpreadsheetName = ws.SelectedSpreadsheet.Title;
                    settings.SpreadsheetURL = ws.SelectedSpreadsheet.Url;
                    settings.WorksheetName = ws.SelectedWorksheet.Title;
                    settings.WorksheetCellsFeedURI = ws.SelectedWorksheet.CellsFeedURI;
                    settings.WorksheetListFeedURI = ws.SelectedWorksheet.ListFeedURI;
                    console.AppendLine(string.Format("Connected to Spreadsheet: {0}/{1}", settings.SpreadsheetName, settings.WorksheetName));
                }
            }

            return false;
        }

        public bool ProcessIntel()
        {
            if (settings.GroupId == null || settings.BattleId == null)
            {
                console.AppendLine(string.Format("{0:G} BOT requires a battle AND intel room connection.", DateTime.Now));
                return false;
            }

            console.AppendLine(string.Format("{0:G} BOT BEGIN", DateTime.Now));
            var result = ProcessWorker();
            console.AppendLine(string.Format("{0:G} BOT END.", DateTime.Now));
            return result;
        }

        public bool ProcessWorker()
        {
            console.AppendLine(string.Format("\t{0:G} Checking for new intel", DateTime.Now));

            var newMessages = TryGetNewMessages();
            if (newMessages == null) return true;
            if (newMessages.Count == 0)
            {
                console.AppendLine(string.Format("\t{0:G} No new intel", DateTime.Now));
                return true;
            }

            console.AppendLine(string.Format("\t{0:G} Processing {1} new message(s)", DateTime.Now, newMessages.Count));
            ProcessNewMessages(newMessages);

            //TODO write all intel to the Sheet!
            //console.Append(string.Format("{0:G} writing new intel to sheet...", DateTime.Now));

            console.AppendLine(string.Format("\t{0:G} Done.", DateTime.Now));
            return true;
        }

        private IList<GroupMessage> TryGetNewMessages()
        {
            string lastMessageId = GetLastIntelReportMessageId();

            int attempts = 0;
            const int maxAttempts = 3;

            while (attempts < 3) 
            { 
                try
                {
                    return groupMe.GroupMessagesByDateRange(settings.GroupId, settings.BattleStartDate, settings.BattleEndDate, lastMessageId);
                }
                catch (Exception ex)
                {
                    console.AppendLine(string.Format("\t{0:G} Error talking to GroupMe: {1}", DateTime.Now, ex.Message));
                    ++attempts;

                    if (attempts < maxAttempts) 
                    {
                        console.AppendLine("\tTry again in 7 seconds.");
                        Thread.Sleep(7000);
                    }
                    
                }
            }

            console.AppendLine(string.Format("\t{0:G} Gave up trying to talk to GroupMe after {1} attempts :(", DateTime.Now, attempts));
            return null;
        }

        private string GetLastIntelReportMessageId()
        {
            string lastMessageId = null;

            NH.UsingSession(s =>
            {
                lastMessageId = s.QueryOver<IntelReport>()
                    .Where(x => x.Battle.Id == settings.BattleId.Value)
                    .And(x => x.GroupId == settings.GroupId)
                    .Select(x => x.MessageId)
                    .OrderBy(x => x.CreateDateUTC).Desc
                    .Take(1)
                    .SingleOrDefault<string>();
            });

            return lastMessageId;
        }

        private void ProcessNewMessages(IList<GroupMessage> raw)
        {
            NH.UsingSession(s =>
            {
                var battle = s.Get<Battle>(settings.BattleId.Value);

                foreach (var m in raw)
                {
                    new IntelMessageProcessor(s, battle, m).Process();
                    s.Flush();
                }
            });
        }

        class IntelMessageProcessor
        {
            private readonly ISession Session;
            private readonly Battle Battle;
            private readonly GroupMessage Message;

            public IntelMessageProcessor(ISession session, Battle battle, GroupMessage message)
            {
                this.Session = session;
                this.Battle = battle;
                this.Message = message;
            }

            public void Process()
            {
                var report = SaveReport();
                if (IsDuplicateOfExistingReport(report)) return;

                var nonEmptyLines = report.Text.SplitToNonEmptyLines();
                report.NonEmptyLineCount = nonEmptyLines.Count();
                if (report.NonEmptyLineCount < 2) //assume chat if only 1 line
                {
                    report.IsChat = true;
                    return;
                }

                string teamName;
                nonEmptyLines = nonEmptyLines.RemoveTeamName(out teamName);
                if (string.IsNullOrEmpty(teamName)) //flag as unknown team and generate a name
                {
                    report.IsUnknownTeamName = true;
                    teamName = "Unknown Team " + report.Id;
                };
                var team = GetOrCreateTeam(teamName);

                var reportStats = nonEmptyLines.Select(x => Stat.Parse(x)).Distinct();
                report.ReportStatsCount = reportStats.Count();

                //check for existing battle stats for this team
                var currentTeamStats = GetCurrentTeamStats(team);
                var newStats = reportStats.Except(currentTeamStats);

                report.NewStatsCount = newStats.Count();
                foreach (var stat in newStats)
                {
                    Session.Save(new BattleStat
                    {
                        Battle = Battle,
                        Team = team,
                        IntelReport = report,
                        Stat = stat
                    });
                }
            }

            private IntelReport SaveReport()
            {
                var report = new IntelReport
                {
                    Battle = Battle,
                    MessageId = Message.id,
                    GroupId = Message.group_id,
                    UserId = Message.user_id,
                    UserName = Message.name,
                    CreateDateUTC = Message.created_at.ToUniversalTime(),
                    ReadDateUTC = DateTime.UtcNow,
                    Text = Message.text ?? string.Empty                  
                };
                report.TextHash = ComputeHash(report.Text);

                Session.Save(report);

                return report;
            }

            private bool IsDuplicateOfExistingReport(IntelReport report)
            {
                var existingReport = Session.QueryOver<IntelReport>()
                    .Where(x => x.Battle.Id == Battle.Id)
                    .And(x => x.Id != report.Id)
                    .And(x => x.TextHash == report.TextHash)
                    .OrderBy(x => x.CreateDateUTC).Asc
                    .Take(1)
                    .SingleOrDefault();

                if (existingReport == null) return false;

                report.DuplicateOf = existingReport;
                return true;
            }

            private string ComputeHash(string text)
            {
                var sha = new SHA1CryptoServiceProvider();
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
                return Encoding.UTF8.GetString(hash);
            }

            private Team GetOrCreateTeam(string teamName)
            {
                var existingTeam = Session.QueryOver<Team>()
                    .Where(x => x.Name == teamName)
                    .SingleOrDefault();

                if (existingTeam == null)
                {
                    existingTeam = new Team { Name = teamName };
                    Session.Save(existingTeam);
                }

                return existingTeam;
            }

            private IList<Stat> GetCurrentTeamStats(Team team)
            {
                return Session.QueryOver<BattleStat>()
                    .Where(x => x.Battle.Id == Battle.Id)
                    .And(x => x.Team.Id == team.Id)
                    .Select(x => x.Stat)
                    .List<Stat>();
            }
        }
    }

    interface IIntelBotConsole
    {
        void AppendLine(string s);
        void Append(string s);
    }

    class IntelBotSettings
    {
        public int? BattleId { get; set; }
        public string BattleName { get; set; }
        public DateTime BattleStartDate { get; set; }
        public DateTime BattleEndDate { get; set; }

        public string GroupId { get; set; }
        public string GroupName { get; set; }

        public string SpreadsheetName { get; set; }
        public string SpreadsheetURL { get; set; }
        public string WorksheetName { get; set; }
        public string WorksheetCellsFeedURI { get; set; }
        public string WorksheetListFeedURI { get; set; }

        public IntelBotSettings Clone()
        {
            return (IntelBotSettings)this.MemberwiseClone();
        }
    }

    //TODO query intelreports and the related BattleStats to display some aggregate stats for the intel room
    //class IntelBotSummaryData
    //{
    //    public int flaggedAsChat { get; set; }
    //    public int unknownTeamNames { get; set; }
    //    public int validReports { get; set; }
    //    public int teams { get; set; }
    //    public int stats { get; set; }
    //}
}
