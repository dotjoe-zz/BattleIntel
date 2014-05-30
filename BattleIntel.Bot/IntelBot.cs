using BattleIntel.Core;
using GroupMe;
using GroupMe.Models;
using GSheet;
using GSheet.Models;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace BattleIntel.Bot
{
    class IntelBot
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(IntelBot));
        private readonly IIntelBotConsole console;
        private IntelBotSettings settings;
        private GroupMeService groupMe;
        private GSheetService google;

        public bool PostSheetURL { get; set; }

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

                    LogInfo("Connected to Battle: {0} {1}", settings.GetBattleDescription(), settings.GetBattleDatesDescription());
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

                    LogInfo("Connected to GroupMeRoom: " + settings.GetGroupDescription());
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

                    LogInfo("Connected to Spreadsheet: " + settings.GetSheetDescription());
                    return true;
                }
            }

            return false;
        }

        public bool Process()
        {
            if (settings.GroupId == null || settings.BattleId == null)
            {
                LogInfo("BOT requires a battle AND intel room connection.");
                return false;
            }

            LogInfo("BOT BEGIN");

            bool continueProcessing = true;
            try
            {
                continueProcessing = ProcessWorker();
            }
            catch (Exception ex)
            {
                LogError("ProcessWorker", ex);
            }
           
            LogInfo("BOT END");
            return continueProcessing;
        }

        private bool ProcessWorker()
        {
            LogProcessInfo("Checking for new intel");

            var newMessages = TryGetNewMessages();
            if (newMessages == null) return true;
            if (newMessages.Count == 0)
            {
                LogProcessInfo("No new intel");
                return true;
            }

            LogProcessInfo("Processing {0} message(s)", newMessages.Count);
            
            var results = ProcessNewMessages(newMessages);

            LogProcessInfo("Parsed {0} new stats", results.NewStatsCount);

            if (results.NewStatsCount > 0 && settings.SpreadsheetURL != null) 
            {
                LogProcessInfo("Updating spreadsheet");
                TryUpdateIntelSheet();
            }

            if (!results.LastMessageWasBot && settings.SpreadsheetURL != null) 
            {
                if (PostSheetURL) 
                { 
                    LogProcessInfo("Posting spreadsheet url");
                    TryPostSpeadsheetURL();
                }
                else
                {
                    LogProcessInfo("Would have posted spreadsheet url but it is DISABLED");
                }
            }

            return true;
        }

        private IList<GroupMessage> TryGetNewMessages()
        {
            string lastMessageId = GetLastNonBotGroupMessageId();

            try
            {
                return groupMe.GroupMessagesByDateRange(settings.GroupId, settings.BattleStartDate, settings.BattleEndDate, lastMessageId);
            }
            catch (Exception ex)
            {
                LogError("GroupMessagesByDateRange", ex);
            }

            return null;
        }

        private string GetLastNonBotGroupMessageId()
        {
            string lastMessageId = null;

            NH.UsingSession(s =>
            {
                lastMessageId = s.QueryOver<IntelReport>()
                    .Where(x => x.Battle.Id == settings.BattleId.Value)
                    .And(x => x.GroupId == settings.GroupId)
                    .And(x => x.IsBotMessage == false)
                    .Select(x => x.MessageId)
                    .OrderBy(x => x.CreateDateUTC).Desc
                    .Take(1)
                    .SingleOrDefault<string>();
            });

            return lastMessageId;
        }

        private ProcessResults ProcessNewMessages(IList<GroupMessage> raw)
        {
            var results = new ProcessResults();

            //process messages in batches to reduce the size of the Transactions
            //and have less loss due to an error
            const int batchSize = 10;
            for (int start = 0; start < raw.Count; start += batchSize)
            { 
                NH.UsingSession(s =>
                {
                    var battle = s.Get<Battle>(settings.BattleId.Value);

                    for (int i = start; i < raw.Count && i < (start + batchSize); ++i)
                    {
                        var p = new IntelReportProcessor(s, battle, raw[i]);
                        
                        p.Process();

                        results.NewStatsCount += p.NewStatsCount;
                        results.LastMessageWasBot = p.IsBotMessage;

                        s.Flush();
                    }
                });
            }

            return results;
        }

        class ProcessResults
        {
            public int NewStatsCount { get; set; }
            public bool LastMessageWasBot { get; set; }
        }

        private void TryUpdateIntelSheet()
        {
            IList<BattleStat> allStats = null;

            NH.UsingSession(s =>
            {
                Team teamAlias = null;

                allStats = s.QueryOver<BattleStat>()
                    .JoinAlias(x => x.Team, () => teamAlias)
                    .Where(x => x.Battle.Id == settings.BattleId.Value)
                    .And(x => !x.IsDeleted)
                    .OrderBy(x => x.Team).Asc
                    .ThenBy(x => x.Stat.Level).Desc
                    .ThenBy(x => x.Stat.DefenseValue).Desc
                    .ThenBy(x => x.Stat.Name).Asc
                    .List();
            });

            var sheetData = allStats.GroupBy(x => new { TeamId = x.Team.Id, TeamName = x.Team.Name })
                .Select(k => new IntelDataRow
                {
                    Team = k.Key.TeamName,
                    Stats = k.Key.TeamName + "\n" + string.Join("\n", k.Select(x => x.Stat)
                            .OrderByDescending(x => x.Level)
                            .ThenByDescending(x => x.DefenseValue)
                            .ThenBy(x => x.Name)
                            .Select(x => x.ToLine()))
                }).OrderBy(x => x.Team).ToList();


            try
            {
                google.MergeSheet(settings.WorksheetCellsFeedURI, settings.WorksheetListFeedURI, sheetData);
            }
            catch (Exception ex)
            {
                LogError("MergeSheet", ex);
            }
        }

        private void TryPostSpeadsheetURL()
        {
            GroupMessage botMessage = null;
            try
            {
                botMessage = groupMe.PostGroupMessage(settings.GroupId, settings.SpreadsheetURL);
            }
            catch (Exception ex)
            {
                LogError("PostGroupMessage", ex);
            }

            if (botMessage != null) 
            { 
                NH.UsingSession(s =>
                {
                    var battle = s.Get<Battle>(settings.BattleId.Value);
                    new IntelReportProcessor(s, battle, botMessage).ProcessBotPost();
                });
            }
        }

        private void LogInfo(string format, params object[] args)
        {
            var text = string.Format(format, args);
            log.Info(text);
            console.AppendLine(string.Format("{0:G} ", DateTime.Now) + text);
        }

        private void LogProcessInfo(string format, params object[] args)
        {
            string text = string.Format(format, args);
            //if(args != null && args.Length > 0) text = 
            log.Info(text);
            console.AppendLine(string.Format("\t{0:G} ", DateTime.Now) + text);
        }

        private void LogError(string message, Exception ex)
        {
            log.Error(message, ex);
            console.AppendLine(ex.Message);
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

        public string GetBattleDescription()
        {
            return string.Format("{0}({1})", BattleName, BattleId);
        }

        public string GetBattleDatesDescription()
        {
            return string.Format("{0:G} - {1:G}", BattleStartDate, BattleEndDate);
        }

        public string GetGroupDescription()
        {
            return string.Format("{0}({1})", GroupName, GroupId);
        }

        public string GetSheetDescription()
        {
            return string.Format("{0} - {1}", SpreadsheetName, WorksheetName);
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
