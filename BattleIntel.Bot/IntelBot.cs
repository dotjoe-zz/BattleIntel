using GSheet;
using GroupMe;
using GroupMe.Models;
using System;
using System.Configuration;
using System.Threading;
using System.Windows.Forms;
using GSheet.Models;
using BattleIntel.Core;
using System.Collections.Generic;
using NHibernate;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BattleIntel.Bot
{
    class IntelBot
    {
        private readonly IIntelBotConsole console;

        private int? battleId;
        private Group intelRoom;
        private SpreadsheetModel spreadsheet;
        private WorksheetModel worksheet;

        private GroupMeService groupMe;
        private GSheetService google;

        public IntelBot(IIntelBotConsole console) 
        { 
            this.console = console;
        }

        public bool ConnectToBattle(IWin32Window owner)
        {
            using (var bs = new BattleSelector())
            {
                if (bs.ShowDialog(owner) == DialogResult.OK)
                {
                    battleId = bs.SelectedBattleId;
                    console.AppendLine(string.Format("Connected to BattleId: {0}", battleId));
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
                    intelRoom = gs.SelectedGroup;
                    console.AppendLine(string.Format("Connected to Intel Room: {0} (id:{1})", intelRoom.name, intelRoom.id));
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
                    spreadsheet = ws.SelectedSpreadsheet;
                    worksheet = ws.SelectedWorksheet;
                    console.AppendLine(string.Format("Connected to Spreadsheet: {0}:{1} {2}", spreadsheet.Title, worksheet.Title, spreadsheet.Url));
                }
            }

            return false;
        }

        public bool ProcessIntel()
        {
            if (intelRoom == null || battleId == null)
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

            var data = LoadBattleData();
            var newMessages = TryGetNewMessages(data);
            if (newMessages == null) return true;
            if (newMessages.Count == 0)
            {
                console.AppendLine(string.Format("\t{0:G} No new intel", DateTime.Now));
                return true;
            }

            console.AppendLine(string.Format("\t{0:G} Processing {1} new message(s)", DateTime.Now, newMessages.Count));
            ProcessNewMessages(newMessages);

            //console.Append(string.Format("{0:G} writing new intel to sheet...", DateTime.Now));

            console.AppendLine(string.Format("\t{0:G} Done.", DateTime.Now));
            return true;
        }

        private BotData LoadBattleData()
        {
            var data = new BotData 
            { 
                BattleId = battleId.Value, 
                GroupId = intelRoom.id 
            };

            NH.UsingSession(s =>
            {
                var b = s.Get<Battle>(data.BattleId);
                data.StartDateUTC = b.StartDateUTC;
                data.EndDateUTC = b.EndDateUTC;
                data.LastIntelReportMessageId = s.QueryOver<IntelReport>()
                    .Where(x => x.Battle.Id == data.BattleId && x.GroupId == data.GroupId)
                    .Select(x => x.MessageId)
                    .OrderBy(x => x.CreateDateUTC).Desc
                    .Take(1)
                    .SingleOrDefault<string>();
            });

            return data;
        }

        private IList<GroupMessage> TryGetNewMessages(BotData data)
        {
            int attempts = 0;
            while (attempts < 3) 
            { 
                try
                {
                    return groupMe.GroupMessagesByDateRange(data.GroupId, data.StartDateUTC, data.EndDateUTC, data.LastIntelReportMessageId);
                }
                catch (Exception ex)
                {
                    console.AppendLine(string.Format("\t{0:G} Error talking to GroupMe: {1}", DateTime.Now, ex.Message));
                    Thread.Sleep(5000);
                    ++attempts;
                }
            }

            console.AppendLine(string.Format("\t{0:G} Gave up trying to talk to GroupMe after {1} attempts :(", DateTime.Now, attempts));
            return null;
        }

        private void ProcessNewMessages(IList<GroupMessage> raw)
        {
            NH.UsingSession(s =>
            {
                foreach (var m in raw)
                {
                    var battle = s.Get<Battle>(battleId.Value);
                    ProcessNewMessage(s, battle, m);
                    s.Flush();
                }
            });
        }

        private void ProcessNewMessage(ISession s, Battle battle, GroupMessage m)
        {
            var textHash = ComputeHash(m.text);
                    
            //first check if this is duplicate Text before we save it...
            var existingReport = s.QueryOver<IntelReport>()
                .Where(x => x.Battle.Id == battle.Id)
                .And(x => x.TextHash == textHash)
                .OrderBy(x => x.CreateDateUTC).Asc
                .Take(1)
                .SingleOrDefault();

            var report = new IntelReport
            {
                Battle = battle,
                MessageId = m.id,
                GroupId = m.group_id,
                UserId = m.user_id,
                UserName = m.name,
                CreateDateUTC = m.created_at.ToUniversalTime(),
                ReadDateUTC = DateTime.UtcNow,
                Text = m.text,
                TextHash = textHash
            };
            s.Save(report);

            if (existingReport != null)
            {
                report.DuplicateOf = existingReport;
                return; //we still save the duplicate so it won't be re-processed
            }

            var lines = m.text.SplitToNonEmptyLines();

            report.NonEmptyLineCount = lines.Count();
            if (report.NonEmptyLineCount < 2)
            {
                report.IsChat = true;
                return; //assume chat if only 1 line
            }

            string teamName;
            lines = lines.RemoveTeamName(out teamName);

            if(string.IsNullOrEmpty(teamName))
            {
                report.IsUnknownTeamName = true;
                teamName = "Unknown Team " + report.Id;
            };

            var team = GetorCreateTeam(s, teamName);
            var stats = lines.Select(x => Stat.Parse(x)).Distinct();

            report.DistinctStatCount = stats.Count();

            foreach (var stat in stats)
            {
                s.Save(new BattleStat
                {
                    Battle = battle,
                    Team = team,
                    IntelReport = report,
                    Stat = stat
                });
            }
        }

        private string ComputeHash(string text)
        {
            var sha = new SHA1CryptoServiceProvider();
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
            return Encoding.UTF8.GetString(hash);
        }

        private Team GetorCreateTeam(ISession s, string teamName)
        {
            var existingTeam = s.QueryOver<Team>()
                .Where(x => x.Name == teamName)
                .SingleOrDefault();

            if (existingTeam == null)
            {
                existingTeam = new Team { Name = teamName };
                s.Save(existingTeam);
            }

            return existingTeam;
        }

        class BotData
        {
            public int BattleId { get; set; }
            public DateTime StartDateUTC { get; set; }
            public DateTime EndDateUTC { get; set; }
            public string GroupId { get; set; }
            public string LastIntelReportMessageId { get; set; }
        }

        //TODO query intelreports and the related BattleStats to display some aggregate stats for the intel room
        class BotSummaryData
        {
            public int flaggedAsChat { get; set; }
            public int unknownTeamNames { get; set; }
            public int validReports { get; set; }
            public int teams { get; set; }
            public int stats { get; set; }
        }
    }

    interface IIntelBotConsole
    {
        void AppendLine(string s);
        void Append(string s);
    }
}
