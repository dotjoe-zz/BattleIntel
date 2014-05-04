using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleIntel.Core
{
    public class IntelReport : Entity
    {
        public virtual string GroupId { get; set; }
        public virtual string MessageId { get; set; }
        public virtual DateTime CreateDateUTC { get; set; }
        public virtual string UserName { get; set; }
        public virtual string UserId { get; set; }
        public virtual string Text { get; set; }
        public virtual DateTime ReadDateUTC { get; set; }

        public virtual IEnumerable<BattleStat> ParsedStats { get; protected set; }

        public IntelReport()
        {
            ParsedStats = new Iesi.Collections.Generic.HashedSet<BattleStat>();
        }
    }
}
