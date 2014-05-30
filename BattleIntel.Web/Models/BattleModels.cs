using BattleIntel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BattleIntel.Web.Models
{
    public class BattleMini
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class BattleHeader
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

    public class TeamIntelHeader
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumReports { get; set; }
        public int NumStats { get; set; }
        public DateTime MostRecentReportUTC { get; set; }
    }

    public class TeamIntelDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<IntelReportMini> Reports { get; set; }
        public IList<BattleStatMini> Stats { get; set; }
    }

    public class IntelReportMini 
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime CreateDateUTC { get; set; }
        public string Text { get; set; }
        public string UpdatedText { get; set; }
        public int NewStatsCount { get; set; }
    }

    public class BattleStatMini
    {
        public int Id { get; set; }
        public int? Level { get; set; }
        public string Name { get; set; }
        public string Defense { get; set; }
        public string AdditionalInfo { get; set; }
        public bool IsDeleted { get; set; }
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