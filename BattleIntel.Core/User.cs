using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleIntel.Core
{
    public class User : Entity
    {
        /// <summary>
        /// The displayed name of the user. NOT UNIQUE!
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Never shown to anyone but this User. Only used for system notifications.
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Date of the User's first Login
        /// </summary>
        public virtual DateTime JoinDateUTC { get; set; }
    }
}
