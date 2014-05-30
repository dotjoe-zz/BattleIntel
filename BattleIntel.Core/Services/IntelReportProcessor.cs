using BattleIntel.Core;
using GroupMe;
using GroupMe.Models;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BattleIntel.Core.Services
{
    public class IntelReportProcessor
    {
        private readonly ISession Session;
        private readonly Battle Battle;
        
        /// <summary>
        /// new stats count is incremented while processing a message
        /// </summary>
        public int NewStatsCount { get; private set; }

        public IntelReportProcessor(ISession session, Battle battle)
        {
            this.Session = session;
            this.Battle = battle;
        }

        /// <summary>
        /// Use this to save the return result from posting a bot message, so we
        /// can flag it as such.
        /// </summary>
        public void ProcessBotPost(GroupMessage message)
        {
            var report = Session.QueryOver<IntelReport>()
                .Where(x => x.Battle.Id == Battle.Id)
                .And(x => x.MessageId == message.id)
                .Take(1)
                .SingleOrDefault();

            if (report != null) return; //already processed! wow!

            report = SaveReport(message);
            report.IsBotMessage = true;
        }

        public void ProcessMessage(GroupMessage message, out bool isBot)
        {
            isBot = false;

            var report = Session.QueryOver<IntelReport>()
                .Where(x => x.Battle.Id == Battle.Id)
                .And(x => x.MessageId == message.id)
                .Take(1)
                .SingleOrDefault();
            
            if (report != null)
            {
                isBot = report.IsBotMessage;
                return; //already processed! we are reading our posted bot message
            }

            report = SaveReport(message);

            var duplicate = GetDuplicateBattleReport(report);
            if (duplicate != null) //this is an exact duplicate of a report we already processed
            {
                report.DuplicateOf = duplicate;
                return;
            }

            ParseReportText(report);
        }

        private void ParseReportText(IntelReport report)
        {
            bool hadTruncatedLine;
            var Text = report.UpdatedText ?? report.Text;
            var nonEmptyLines = Text.SplitToNonEmptyLines(255, out hadTruncatedLine);

            report.HadTruncatedLine = hadTruncatedLine;
            report.NonEmptyLineCount = nonEmptyLines.Count();
            report.IsChat = report.NonEmptyLineCount < 2; //assume chat if only 1 line

            if (report.IsChat || report.HadTruncatedLine) return;

            string teamName;
            nonEmptyLines = nonEmptyLines.RemoveTeamName(out teamName);
            if (string.IsNullOrEmpty(teamName)) //flag as unknown team and generate a name
            {
                report.IsUnknownTeamName = true;
                teamName = "Unknown Team " + report.Id;
            };
            var team = GetOrCreateTeam(teamName);
            report.Team = team;

            var reportStats = nonEmptyLines.Select(x => Stat.Parse(x)).Distinct();
            report.ReportStatsCount = reportStats.Count();

            //check for existing battle stats for this team
            var currentTeamStats = GetCurrentTeamStats(team);
            var newStats = reportStats.Except(currentTeamStats);

            report.NewStatsCount = newStats.Count();

            this.NewStatsCount += report.NewStatsCount;

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

        public void ReParseReportText(IntelReport report)
        {
            //reset all stats/counters/flags
            report.HadTruncatedLine = false;
            report.NonEmptyLineCount = 0;
            report.IsChat = false;

            report.ReportStatsCount = 0;
            report.NewStatsCount = 0;
            report.IsUnknownTeamName = false;
            report.Team = null;
            report.Stats.Clear();

            //and re-parse
            ParseReportText(report);
        }

        private IntelReport SaveReport(GroupMessage message)
        {
            var report = new IntelReport
            {
                Battle = Battle,
                MessageId = message.id,
                GroupId = message.group_id,
                UserId = message.user_id,
                UserName = message.name,
                CreateDateUTC = message.created_at.ToUniversalTime(),
                ReadDateUTC = DateTime.UtcNow,
                Text = message.text ?? string.Empty
            };
            report.TextHash = ComputeHash(report.Text);

            Session.Save(report);

            return report;
        }

        private IntelReport GetDuplicateBattleReport(IntelReport report)
        {
            return Session.QueryOver<IntelReport>()
                .Where(x => x.Battle.Id == Battle.Id)
                .And(x => x.Id != report.Id)
                .And(x => x.TextHash == report.TextHash)
                .OrderBy(x => x.CreateDateUTC).Asc
                .Take(1)
                .SingleOrDefault();
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

        /// <summary>
        /// Get ALL current stats for this team. Even includes Deleted stats so they don't get re-added.
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
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
