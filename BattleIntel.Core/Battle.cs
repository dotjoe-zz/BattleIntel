using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleIntel.Core
{
    public class Battle : Entity
    {
        public virtual string Name { get; set; }
        public virtual DateTime StartDateUTC { get; set; }
        public virtual DateTime EndDateUTC { get; set; }

        public virtual Iesi.Collections.Generic.ISet<IntelReport> Reports { get; protected set; }
        public virtual Iesi.Collections.Generic.ISet<BattleStat> Stats { get; protected set; }

        public Battle()
        {
            Reports = new Iesi.Collections.Generic.HashedSet<IntelReport>();
            Stats = new Iesi.Collections.Generic.HashedSet<BattleStat>();
        }
    }
}
