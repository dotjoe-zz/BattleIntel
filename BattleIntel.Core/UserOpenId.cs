using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleIntel.Core
{
    public class UserOpenId : Entity
    {
        public virtual User User { get; set; }
        public virtual string OpenIdentifier { get; set; }
    }
}
