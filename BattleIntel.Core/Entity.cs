using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleIntel.Core
{
    public class Entity
    {
        public virtual int Id { get; protected set; }

        private int? requestedHashCode;

        public override bool Equals(object obj)
        {
            var that = obj as Entity;
            if (that == null) return false;
            if (!GetType().IsInstanceOfType(that)) return false;
            if (that == this) return true;

            if (IsTransient(that) && IsTransient(this))
            {
                //already failed reference equality so these transients cannot be equal
                return false;
            }

            return that.Id.Equals(this.Id);
        }

        protected bool IsTransient(Entity entity)
        {
            return Equals(entity.Id, default(int));
        }

        public override int GetHashCode()
        {
            if (!requestedHashCode.HasValue)
            {
                requestedHashCode = IsTransient(this) ? base.GetHashCode() : Id.GetHashCode();
            }
            return requestedHashCode.Value;
        }
    }
}
