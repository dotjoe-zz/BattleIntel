using System;
using NHibernate;
using NHibernate.Type;

namespace BattleIntel.Core.Db
{
    class UtcDateTimeInterceptor : IInterceptor
    {
        public bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            ConvertDatabaseDateTimeToUtc(state, types);
            return true;
        }

        public bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            ConvertLocalDateToUtc(state, types);
            return true;
        }

        public bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
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

        public void OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types) { }
        public int[] FindDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types) { return null; }
        public object Instantiate(Type type, object id) { return null; }
        public object IsUnsaved(object entity) { return null; }
        public void PreFlush(System.Collections.ICollection entities) { }
        public void PostFlush(System.Collections.ICollection entities) { }
        public void AfterTransactionBegin(ITransaction tx) { }
        public void AfterTransactionCompletion(ITransaction tx) { }
        public void BeforeTransactionCompletion(ITransaction tx) { }
        public object GetEntity(string entityName, object id) { return null; }
        public string GetEntityName(object entity) { return null; }
        public object Instantiate(string entityName, EntityMode entityMode, object id) { return null; }
        public bool? IsTransient(object entity) { return null; }
        public void OnCollectionRecreate(object collection, object key) { }
        public void OnCollectionRemove(object collection, object key) { }
        public void OnCollectionUpdate(object collection, object key) { }
        public NHibernate.SqlCommand.SqlString OnPrepareStatement(NHibernate.SqlCommand.SqlString sql) { return sql; }
        public void SetSession(ISession session) { }
    }
}
