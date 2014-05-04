using System;
using NHibernate;
using NHibernate.Type;

namespace BattleIntel.Core.Db
{
    class UtcDateTimeInterceptor : EmptyInterceptor
    {
        public override bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            ConvertDatabaseDateTimeToUtc(state, types);
            return true;
        }

        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            ConvertLocalDateToUtc(state, types);
            return true;
        }

        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
        {
            ConvertLocalDateToUtc(currentState, types);
            return true;
        }

        private void ConvertLocalDateToUtc(object[] state, IType[] types)
        {
            int index = 0;
            foreach (IType type in types)
            {
                if ((type.ReturnedClass == typeof(DateTime)) && state[index] != null && (((DateTime)state[index]).Kind == DateTimeKind.Local))
                {
                    state[index] = ((DateTime)state[index]).ToUniversalTime();
                }
                else if ((type.ReturnedClass == typeof(Nullable<DateTime>)) && state[index] != null && (((DateTime?)state[index]).Value.Kind == DateTimeKind.Local))
                {
                    DateTime? result = ((DateTime?)state[index]).Value.ToUniversalTime();
                    state[index] = result;
                }

                ++index;
            }
        }

        private void ConvertDatabaseDateTimeToUtc(object[] state, IType[] types)
        {
            int index = 0;
            foreach (IType type in types)
            {
                if ((type.ReturnedClass == typeof(DateTime)) && state[index] != null && (((DateTime)state[index]).Kind != DateTimeKind.Utc))
                {
                    //Create a new date and assume the value stored in the database is Utc
                    DateTime cur = (DateTime)state[index];
                    DateTime result = DateTime.SpecifyKind(cur, DateTimeKind.Utc);
                    state[index] = result;
                }
                else if ((type.ReturnedClass == typeof(Nullable<DateTime>)) && state[index] != null && (((DateTime?)state[index]).Value.Kind != DateTimeKind.Utc))
                {
                    //Create a new date and assume the value stored in the database is Utc
                    DateTime? cur = (DateTime?)state[index];
                    DateTime? result = DateTime.SpecifyKind(cur.Value, DateTimeKind.Utc);
                    state[index] = result;
                }

                ++index;
            }
        }
    }
}
