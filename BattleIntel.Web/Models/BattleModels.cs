using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BattleIntel.Web.Models
{
    public class BattleSummary
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDateUTC { get; set; }
        public DateTime EndDateUTC { get; set; }
        public int NumUsersReporting { get; set; }
        public int NumReports { get; set; }
        public int NumTeams { get; set; }
        public int NumStats { get; set; }
    }

    public class IntelReportHeader
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime CreateDateUTC { get; set; }
        public string Text { get; set; }
        public int ReportStatsCount { get; set; }
        public int NewStatsCount { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
    }
}