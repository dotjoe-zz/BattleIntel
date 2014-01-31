using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleIntel.Core.Db.Generate
{
    class Program
    {
        static readonly string connectionString = @"Server=localhost\SQLEXPRESS;Initial Catalog=BattleIntel;Integrated Security=SSPI;";

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));

            Configuration nhCfg = null;

            using (var sf = NHibernateConfiguration.BuildSessionFactory(cfg =>
            {
                cfg.DataBaseIntegration(x =>
                {
                    x.ConnectionProvider<NHibernate.Connection.DriverConnectionProvider>();
                    x.Dialect<NHibernate.Dialect.MsSql2012Dialect>();
                    x.Driver<NHibernate.Driver.SqlClientDriver>();
                    x.ConnectionString = connectionString;
                    //x.LogSqlInConsole = true;
                    x.LogFormattedSql = true;
                    x.KeywordsAutoImport = NHibernate.Cfg.Hbm2DDLKeyWords.AutoQuote;
                });
                cfg.SetProperty(NHibernate.Cfg.Environment.DefaultSchema, "dbo");

            }, "../../Schemas", cfg => nhCfg = cfg))
            {
                Console.WriteLine("Connection: " + connectionString);

                if (Confirm("Connection looks good?"))
                {
                    if (Confirm("Drop and Create New Tables?"))
                    {
                        new SchemaExport(nhCfg).Create(true, true);
                    }

                    if (Confirm("Generate Test Data?"))
                    {
                        CreateTestData(sf);
                    }
                }
            }

            Console.WriteLine("Press Any Key to Exit");
            Console.ReadKey();
        }

        static bool Confirm(string question)
        {
            while (true)
            {
                Console.Write(string.Format("{0} (y/n)", question));
                var line = Console.ReadLine();
                if (line == "y") return true;
                if (line == "n") return false;
            }
        }

        static void CreateTestData(ISessionFactory sf)
        {
            using(var s = sf.OpenSession())
            using (var tx = s.BeginTransaction())
            {
                var user = new User 
                { 
                    Username = "system-generated",
                    JoinDateUTC = DateTime.Now
                };

                var battle = new Battle
                {
                    Name = "BattleIntel Throwdown Test",
                    StartDateUTC = DateTime.Now,
                    EndDateUTC = DateTime.Now.AddDays(3),
                    CreatedUTC = DateTime.Now,
                    CreatedBy = user
                };

                var team = new Team
                {
                    Name = "Duck Squad",
                    CreatedUTC = DateTime.Now,
                    CreatedBy = user
                };

                s.Save(user);
                s.Save(battle);
                s.Save(team);

                for (int i = 191; i <= 250; ++i)
                {
                    s.Save(new BattleStat
                    {
                        Battle = battle,
                        Team = team,
                        Stat = new Stat 
                        {
                            Level = i,
                            Name = "Duck-" + i,
                            Defense = (i * 10).ToString() + "k"
                        },
                        CreatedUTC = DateTime.Now,
                        CreatedBy = user
                    });
                }

                tx.Commit();
            }
        }
    }
}
