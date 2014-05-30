using System;

namespace BattleIntel.Core
{
    public class BattleStat : Entity
    {
        public virtual Battle Battle { get; set; }
        public virtual Team Team { get; set; }
        public virtual Stat Stat { get; set; }
        public virtual bool IsDeleted { get; set; }
    }
}
