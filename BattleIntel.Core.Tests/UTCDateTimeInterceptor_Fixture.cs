using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleIntel.Core.Tests
{
    /// <summary>
    /// Need to truncate milliseconds due to SQLite DateTime not having that precision
    /// </summary>
    [TestFixture]
    class UTCDateTimeInterceptor_Fixture : SQLiteScopedFixture
    {
        protected int SaveUser(DateTime joinDate)
        {
            int id = 0;

            UsingSession(s =>
            {
                var u = new User
                {
                    Email = "test@test.com",
                    DisplayName = "test",
                    JoinDateUTC = joinDate
                };
                s.Save(u);
                id = u.Id;
            });

            return id;
        }

        protected int SaveTeam(DateTime? lastUpdated)
        {
            int id = 0;

            UsingSession(s =>
            {
                var u = s.Load<User>(SaveUser(DateTime.Now));

                var t = new Team
                {
                    Name = "test",
                    CreatedBy = u,
                    CreatedUTC = DateTime.UtcNow
                };

                if(lastUpdated.HasValue)
                {
                    t.LastUpdatedBy = u;
                    t.LastUpdatedUTC = lastUpdated;
                }

                s.Save(t);
                id = t.Id;
            });

            return id;
        }

        [Test]
        public void UTC_InsertUTCDate()
        {
            var utcNow = DateTime.UtcNow.TruncateMilliseconds();
            var id = SaveUser(utcNow);

            UsingSession(s =>
            {
                var u = s.Get<User>(id);
                Assert.AreEqual(DateTimeKind.Utc, u.JoinDateUTC.Kind);
                Assert.AreEqual(utcNow, u.JoinDateUTC);
            });
        }

        [Test]
        public void UTC_InsertLocalDate()
        {
            var now = DateTime.Now.TruncateMilliseconds();
            var id = SaveUser(now);

            UsingSession(s =>
            {
                var u = s.Get<User>(id);
                Assert.AreEqual(DateTimeKind.Utc, u.JoinDateUTC.Kind);
                Assert.AreNotEqual(now, u.JoinDateUTC);
                Assert.AreNotEqual(now, now.ToUniversalTime());
                Assert.AreEqual(now.ToUniversalTime(), u.JoinDateUTC);
            });
        }
        
        [Test]
        public void UTC_InsertUTCThenUpdateUTC()
        {
            var utcNow = DateTime.UtcNow.TruncateMilliseconds();
            var id = SaveUser(utcNow);

            var utcJoinDate = utcNow.AddDays(-10).AddHours(-1);

            UsingSession(s =>
            {
                var u = s.Get<User>(id);
                u.JoinDateUTC = utcJoinDate;
            });

            UsingSession(s =>
            {
                var u = s.Get<User>(id);
                Assert.AreEqual(DateTimeKind.Utc, u.JoinDateUTC.Kind);
                Assert.AreEqual(utcJoinDate, u.JoinDateUTC);
            });
        }

        [Test]
        public void UTC_InsertUTCThenUpdateLocal()
        {
            var utcNow = DateTime.UtcNow.TruncateMilliseconds();
            var id = SaveUser(utcNow);

            var joinDate = utcNow.AddDays(-10).AddHours(-1).ToLocalTime();

            UsingSession(s =>
            {
                var u = s.Get<User>(id);
                u.JoinDateUTC = joinDate;
            });

            UsingSession(s =>
            {
                var u = s.Get<User>(id);
                Assert.AreEqual(DateTimeKind.Utc, u.JoinDateUTC.Kind);
                Assert.AreEqual(joinDate.ToUniversalTime(), u.JoinDateUTC);
            });
        }

        [Test]
        public void UTC_InsertLocalThenUpdateUTC()
        {
            var now = DateTime.Now.TruncateMilliseconds();
            var id = SaveUser(now);

            var utcJoinDate = now.AddDays(-10).AddHours(-1).ToUniversalTime();

            UsingSession(s =>
            {
                var u = s.Get<User>(id);
                u.JoinDateUTC = utcJoinDate;
            });

            UsingSession(s =>
            {
                var u = s.Get<User>(id);
                Assert.AreEqual(DateTimeKind.Utc, u.JoinDateUTC.Kind);
                Assert.AreEqual(utcJoinDate, u.JoinDateUTC);
            });
        }

        [Test]
        public void UTC_InsertLocalThenUpdateLocal()
        {
            var now = DateTime.Now.TruncateMilliseconds();
            var id = SaveUser(now);

            var joinDate = now.AddDays(-10).AddHours(-1);

            UsingSession(s =>
            {
                var u = s.Get<User>(id);
                u.JoinDateUTC = joinDate;
            });

            UsingSession(s =>
            {
                var u = s.Get<User>(id);
                Assert.AreEqual(DateTimeKind.Utc, u.JoinDateUTC.Kind);
                Assert.AreEqual(joinDate.ToUniversalTime(), u.JoinDateUTC);
            });
        }

        [Test]
        public void UTCNullable_InsertUTCDate()
        {
            var utcNow = DateTime.UtcNow.TruncateMilliseconds();
            var id = SaveTeam(utcNow);

            UsingSession(s =>
            {
                var t = s.Get<Team>(id);
                Assert.AreEqual(DateTimeKind.Utc, t.LastUpdatedUTC.Value.Kind);
                Assert.AreEqual(utcNow, t.LastUpdatedUTC);
            });
        }

        [Test]
        public void UTCNullable_InsertLocalDate()
        {
            var now = DateTime.Now.TruncateMilliseconds();
            var id = SaveTeam(now);

            UsingSession(s =>
            {
                var t = s.Get<Team>(id);
                Assert.AreEqual(DateTimeKind.Utc, t.LastUpdatedUTC.Value.Kind);
                Assert.AreNotEqual(now, t.LastUpdatedUTC.Value);
                Assert.AreNotEqual(now, now.ToUniversalTime());
                Assert.AreEqual(now.ToUniversalTime(), t.LastUpdatedUTC.Value);
            });
        }

        [Test]
        public void UTCNullable_InsertUTCThenUpdateUTC()
        {
            var utcNow = DateTime.UtcNow.TruncateMilliseconds();
            var id = SaveTeam(utcNow);

            var utcJoinDate = utcNow.AddDays(-10).AddHours(-1);

            UsingSession(s =>
            {
                var t = s.Get<Team>(id);
                t.LastUpdatedUTC = utcJoinDate;
            });

            UsingSession(s =>
            {
                var t = s.Get<Team>(id);
                Assert.AreEqual(DateTimeKind.Utc, t.LastUpdatedUTC.Value.Kind);
                Assert.AreEqual(utcJoinDate, t.LastUpdatedUTC.Value);
            });
        }

        [Test]
        public void UTCNullable_InsertUTCThenUpdateLocal()
        {
            var utcNow = DateTime.UtcNow.TruncateMilliseconds();
            var id = SaveTeam(utcNow);

            var joinDate = utcNow.AddDays(-10).AddHours(-1).ToLocalTime();

            UsingSession(s =>
            {
                var t = s.Get<Team>(id);
                t.LastUpdatedUTC = joinDate;
            });

            UsingSession(s =>
            {
                var t = s.Get<Team>(id);
                Assert.AreEqual(DateTimeKind.Utc, t.LastUpdatedUTC.Value.Kind);
                Assert.AreEqual(joinDate.ToUniversalTime(), t.LastUpdatedUTC.Value);
            });
        }

        [Test]
        public void UTCNullable_InsertLocalThenUpdateUTC()
        {
            var now = DateTime.Now.TruncateMilliseconds();
            var id = SaveTeam(now);

            var utcJoinDate = now.AddDays(-10).AddHours(-1).ToUniversalTime();

            UsingSession(s =>
            {
                var t = s.Get<Team>(id);
                t.LastUpdatedUTC = utcJoinDate;
            });

            UsingSession(s =>
            {
                var t = s.Get<Team>(id);
                Assert.AreEqual(DateTimeKind.Utc, t.LastUpdatedUTC.Value.Kind);
                Assert.AreEqual(utcJoinDate, t.LastUpdatedUTC.Value);
            });
        }

        [Test]
        public void UTCNullable_InsertLocalThenUpdateLocal()
        {
            var now = DateTime.Now.TruncateMilliseconds();
            var id = SaveTeam(now);

            var joinDate = now.AddDays(-10).AddHours(-1);

            UsingSession(s =>
            {
                var t = s.Get<Team>(id);
                t.LastUpdatedUTC = joinDate;
            });

            UsingSession(s =>
            {
                var t = s.Get<Team>(id);
                Assert.AreEqual(DateTimeKind.Utc, t.LastUpdatedUTC.Value.Kind);
                Assert.AreEqual(joinDate.ToUniversalTime(), t.LastUpdatedUTC.Value);
            });
        }
    }

    public static class DateTimeExtensions
    {
        public static DateTime Truncate(this DateTime d, TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.Zero) return d; // Or could throw an ArgumentException
            return d.AddTicks(-(d.Ticks % timeSpan.Ticks));
        }

        public static DateTime TruncateMilliseconds(this DateTime d)
        {
            return d.Truncate(TimeSpan.FromSeconds(1));
        }
    }
}
