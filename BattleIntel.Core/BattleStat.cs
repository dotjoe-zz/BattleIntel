using System;

namespace BattleIntel.Core
{
    public class BattleStat : Entity, IAuditable
    {
        public virtual Battle Battle { get; set; }
        public virtual Team Team { get; set; }
        public virtual Stat Stat { get; set; }

        public virtual DateTime CreatedUTC { get; set; }
        public virtual User CreatedBy { get; set; }
        public virtual DateTime? LastUpdatedUTC { get; set; }
        public virtual User LastUpdatedBy { get; set; }
    }
}
