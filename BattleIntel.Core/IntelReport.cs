using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleIntel.Core
{
    public class IntelReport : Entity
    {
        public virtual Battle Battle { get; set; }
        public virtual string GroupId { get; set; }
        public virtual string MessageId { get; set; }
        public virtual DateTime CreateDateUTC { get; set; }
        public virtual string UserName { get; set; }
        public virtual string UserId { get; set; }
        public virtual string Text { get; set; }
        public virtual string TextHash { get; set; }
        public virtual string UpdatedText { get; set; }
        public virtual DateTime ReadDateUTC { get; set; }
        public virtual bool IsBotMessage { get; set; }
        public virtual IntelReport DuplicateOf { get; set; }
        public virtual bool IsChat { get; set; }
        public virtual int NonEmptyLineCount { get; set; }
        public virtual bool HadTruncatedLine { get; set; }
        public virtual bool IsUnknownTeamName { get; set; }

        /// <summary>
        /// The number of stats in this report text.
        /// </summary>
        public virtual int ReportStatsCount { get; set; }

        /// <summary>
        /// The number of stats that where actually created. (i.e. Stats.Count()).
        /// This is after merging with existing stats.
        /// </summary>
        public virtual int NewStatsCount { get; set; }

        public virtual Team Team { get; set; }
        public virtual IEnumerable<BattleStat> Stats { get; protected set; }

        public IntelReport()
        {
            Stats = new Iesi.Collections.Generic.HashedSet<BattleStat>();
        }
    }
}
