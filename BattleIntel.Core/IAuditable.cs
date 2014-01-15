using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleIntel.Core
{
    public interface IAuditable
    {
        DateTime CreatedUTC { get; set; }
        User CreatedBy { get; set; }
        DateTime? LastUpdatedUTC { get; set; }
        User LastUpdatedBy { get; set; }
    }
}
