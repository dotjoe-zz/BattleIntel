using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleIntel.Core.Tests
{
    [TestFixture]
    class StatEquality_Fixture
    {
        [Test]
        public void StatEquality_AdditionalInfoCaseInsensitve()
        {
            var sBig = new Stat
            {
                Level = 100,
                Name = "Bozo",
                Defense = "15m",
                DefenseValue = 15000000,
                AdditionalInfo = "AddiTional InFO"
            };

            var sSmall = new Stat
            {
                Level = 100,
                Name = "Bozo",
                Defense = "15m",
                DefenseValue = 15000000,
                AdditionalInfo = "additional info"
            };

            Assert.AreEqual(sBig, sSmall);
        }

        [Test]
        public void StatEquality_DefenseValuePreferred()
        {
            Assert.AreEqual(Stat.Parse("100 Bozo 15mil"), Stat.Parse("100 Bozo 15m"));
            Assert.AreEqual(Stat.Parse("100 Bozo 15mil"), Stat.Parse("100 Bozo 15.00m"));
            Assert.AreEqual(Stat.Parse("100 Bozo 15.123mil"), Stat.Parse("100 Bozo 15,123"));
            Assert.AreEqual(Stat.Parse("100 Bozo 15.1234M"), Stat.Parse("100 Bozo 15,1234"));

            var sBig = new Stat
            {
                Level = 100,
                Name = "Bozo",
                Defense = "15mil",
                DefenseValue = 15000000
            };

            var sSmall = new Stat
            {
                Level = 100,
                Name = "Bozo",
                Defense = "15m",
                DefenseValue = 15000000
            };

            Assert.AreEqual(sBig, sSmall);
        }
    }
}
