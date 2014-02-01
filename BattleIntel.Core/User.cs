using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleIntel.Core
{
    public class User : Entity
    {
        public virtual string Email { get; set; }

        /// <summary>
        /// The displayed name of the user. NOT UNIQUE!
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Date of the User's first Login
        /// </summary>
        public virtual DateTime JoinDateUTC { get; set; }
    }
}
