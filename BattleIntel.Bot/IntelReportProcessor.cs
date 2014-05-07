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

namespace BattleIntel.Bot
{
    class IntelReportProcessor
    {
        private readonly ISession Session;
        private readonly Battle Battle;
        private readonly GroupMessage Message;
        public int NewStatsCount { get; private set; }
        public bool IsBotMessage { get; private set; }

        public IntelReportProcessor(ISession session, Battle battle, GroupMessage message)
        {
            this.Session = session;
            this.Battle = battle;
            this.Message = message;
        }

        /// <summary>
        /// Use this to save the return result from posting a bot message, so we
        /// can flag it as such.
        /// </summary>
        public void ProcessBotPost()
        {
            var report = Session.QueryOver<IntelReport>()
                .Where(x => x.Battle.Id == Battle.Id)
                .And(x => x.MessageId == Message.id)
                .Take(1)
                .SingleOrDefault();

            if (report != null)
            {
                IsBotMessage = report.IsBotMessage;
                return; //already processed! wow!
            }

            report = SaveReport();

            report.IsBotMessage = true;
            IsBotMessage = true;
        }

        public void Process()
        {
            var report = Session.QueryOver<IntelReport>()
                .Where(x => x.Battle.Id == Battle.Id)
                .And(x => x.MessageId == Message.id)
                .Take(1)
                .SingleOrDefault();
            if (report != null)
            {
                IsBotMessage = report.IsBotMessage;
                return; //already processed! most likely we are reading our posted bot message
            }

            report = SaveReport();

            var duplicate = GetDuplicateBattleReport(report);
            if (duplicate != null) //this is an exact duplicate of a report we already processed
            {
                report.DuplicateOf = duplicate;
                return;
            }

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
            this.NewStatsCount = report.NewStatsCount;

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
