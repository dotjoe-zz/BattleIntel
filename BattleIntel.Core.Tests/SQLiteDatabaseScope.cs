using BattleIntel.Core.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using System;
using System.Data.SQLite;
using System.Diagnostics;

namespace BattleIntel.Core.Tests
{
    /// <summary>
    /// SQLite in memory session factory abstraction for testing
    /// http://jasondentler.com/blog/2009/08/nhibernate-unit-testing-with-sqlite-in-memory-db/
    /// </summary>
    /// <typeparam name="TClassFromMappingAssembly"></typeparam>
    public class SQLiteDatabaseScope : IDisposable
    {
        private const string CONNECTION_STRING = "Data Source=:memory:;Version=3;New=True;";

        public SQLiteDatabaseScope()
        {
            BuildConfiguration();
        }

        private SQLiteConnection m_Connection;
        private ISessionFactory m_SessionFactory;

        public void BuildConfiguration()
        {
            m_SessionFactory = NHibernateConfiguration.BuildSessionFactory(configureByCode: ConfigureByCode, exposeCfg: BuildSchema);
        }

        private void ConfigureByCode(NHibernate.Cfg.Configuration cfg)
        {
            cfg.DataBaseIntegration(db =>
            {
                db.ConnectionProvider<NHibernate.Connection.DriverConnectionProvider>();
                db.Dialect<NHibernate.Dialect.SQLiteDialect>();
                db.Driver<NHibernate.Driver.SQLite20Driver>();
                db.ConnectionString = CONNECTION_STRING;
                db.LogSqlInConsole = true;
                db.LogFormattedSql = true;
            });
        }

        private void BuildSchema(NHibernate.Cfg.Configuration Cfg)
        {
            SchemaExport SE = new SchemaExport(Cfg);
            SE.Execute(false, true, false, GetConnection(), Console.Out);
        }

        private System.Data.SQLite.SQLiteConnection GetConnection()
        {
            if (m_Connection == null)
            {
                m_Connection = new SQLiteConnection(CONNECTION_STRING);
                m_Connection.Open();
            }
            return m_Connection;
        }

        public ISession OpenSession()
        {
            return m_SessionFactory.OpenSession(GetConnection());
        }

        #region " IDisposable Support "

        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        // To detect redundant calls
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    if (m_Connection != null) m_Connection.Close();
                    m_Connection = null;

                }
            }

            this.disposedValue = true;
        }
    }

    /// <summary>
    /// NUnit TestFixture that setups a new SQLiteDatabaseScope for each test.
    /// </summary>
    [TestFixture]
    public abstract class SQLiteScopedFixture
    {
        protected SQLiteDatabaseScope scope;

        [SetUp]
        public void Initialize()
        {
            Debug.WriteLine("[new scope]");
            scope = new SQLiteDatabaseScope();
        }

        [TearDown]
        public void Cleanup()
        {
            Debug.WriteLine("[dispose scope]");
            scope.Dispose();
        }

        protected void UsingSession(Action<ISession> doWork)
        {
            using (var s = scope.OpenSession())
            {
                s.FlushMode = FlushMode.Commit;

                using (var tx = s.BeginTransaction())
                {
                    doWork(s);
                    tx.Commit();
                }
            }
        }
    }
}
