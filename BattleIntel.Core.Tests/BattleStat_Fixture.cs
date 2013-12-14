using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleIntel.Core.Tests
{
    [TestFixture]
    public class BattleStat_Fixture
    {
        [Test]
        public void Stat_Parsing()
        {
            Assert.AreEqual("250 Name 1.23", BattleStat.Parse("Name lvl 250 1.23 m").ToString());
            Assert.AreEqual("250 Name 1.23m", BattleStat.Parse("Name L250 1.23m").ToString());
            Assert.AreEqual("250 Name 1.23m", BattleStat.Parse("Name/250/1.23m").ToString());
            Assert.AreEqual("250 Name 1.23m", BattleStat.Parse("250 1,23m Name").ToString());
            Assert.AreEqual("250 Name 1.23m", BattleStat.Parse("250,1.23m,Name").ToString());
            Assert.AreEqual("250 Name 1.23m", BattleStat.Parse("250 - 1.23m - Name").ToString());
            Assert.AreEqual("250 Name 1.23m", BattleStat.Parse("Name 250 1,23m").ToString());
        }
    }
}
