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
    }
}
