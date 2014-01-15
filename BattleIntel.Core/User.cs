using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleIntel.Core
{
    public class User : Entity
    {
        public virtual string Username { get; set; }
        public virtual DateTime JoinDateUTC { get; set; }
        public virtual DateTime LastSeenDateUTC { get; set; }
    }
}
