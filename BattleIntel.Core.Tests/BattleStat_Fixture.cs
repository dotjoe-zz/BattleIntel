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
            AssertStatParse("Name lvl 250 1.23 m", 250, "Name", "1.23");

            AssertStatParse("Name L250 1.23m", 250, "Name", "1.23m");
            AssertStatParse("Name/250/1.23m", 250, "Name", "1.23m");
            AssertStatParse("250 1,23m Name", 250, "Name", "1.23m");
            AssertStatParse("250,1.23m,Name", 250, "Name", "1.23m");
            AssertStatParse("250 - 1.23m - Name", 250, "Name", "1.23m");
            AssertStatParse("Name 250 1,23m", 250, "Name", "1.23m");
        }

        private void AssertStatParse(string input, int level, string name, string defense)
        {
            var expected = new BattleStat
            {
                Level = level,
                Name = name,
                Defense = defense
            };

            Assert.AreEqual(expected.ToString(), BattleStat.Parse(input).ToString());
        }
    }
}
