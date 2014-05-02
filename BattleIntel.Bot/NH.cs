using BattleIntel.Core;
using BattleIntel.Core.Db;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleIntel.Bot
{
    static class NH
    {
        private static ISessionFactory sessionFactory = NHibernateConfiguration.BuildSessionFactory();

        public static void UsingSession(Action<ISession> dbWork)
        {
            using (var s = sessionFactory.OpenSession())
            using (var tx = s.BeginTransaction())
            {
                dbWork(s);
                tx.Commit();
            }
        }

        public static User GetBotUser(ISession s)
        {
            const string botEmail = "bot@battleintel.com";
            var u = s.QueryOver<User>().Where(x => x.Email == botEmail).SingleOrDefault();
            if (u != null) return u;
            u = new User
            {
                DisplayName = "Battle Intel Bot",
                Email = botEmail,
                JoinDateUTC = DateTime.UtcNow
            };
            s.Save(u);
            return u;
        }
    }
}
