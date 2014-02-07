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
        public void StatParse_Standard()
        {
            AssertStatParse("Name 250 1.23m", 250, "Name", "1.23m");
            AssertStatParse("250 Name 1.23m", 250, "Name", "1.23m");
            AssertStatParse("250 1.23m Name", 250, "Name", "1.23m");
        }

        [Test]
        public void StatParse_MissingPieces()
        {
            AssertStatParse("250 1.23m", 250, null, "1.23m");
            AssertStatParse("250 Name", 250, "Name", null);
            AssertStatParse("Name 1.23m", null, "Name", "1.23m");
        }

        [Test]
        public void StatParse_JunkIndicators()
        {
            AssertStatParse("Name lvl 250 def 1.23", 250, "Name", "1.23");
            AssertStatParse("Name lv 250 d 1.23", 250, "Name", "1.23");
            AssertStatParse("Name l250 d1.23", 250, "Name", "1.23");
            AssertStatParse("Name L250 D1.23", 250, "Name", "1.23");
            AssertStatParse("Name Lvl250 Def1.23", 250, "Name", "1.23");
            AssertStatParse("Name Lv250 def1.23", 250, "Name", "1.23");
        }

        [Test]
        public void StatParse_Canadian()
        {
            AssertStatParse("Name 250 1,2m", 250, "Name", "1.2m");
            AssertStatParse("Name 250 1,23m", 250, "Name", "1.23m");
            AssertStatParse("Name 250 1,23M", 250, "Name", "1.23M");

            AssertStatParse("250 1,2m Name", 250, "Name", "1.2m");
            AssertStatParse("250 1,23m Name", 250, "Name", "1.23m");
            AssertStatParse("250 1,23M Name", 250, "Name", "1.23M");

            AssertStatParse("Name 250 1,234k", 250, "Name", "1234k"); //not really Canadian, we just removed the thousandth's separator
        }

        [Test]
        public void StatParse_Compressed()
        {
            AssertStatParse("Name/250/1.23m", 250, "Name", "1.23m");
            AssertStatParse("250,1.23m,Name", 250, "Name", "1.23m");
            AssertStatParse("250-1.23m-Name", 250, "Name", "1.23m");
            AssertStatParse("250-1,23m-Name", 250, "Name", "1.23m"); //compressed Canadian
        }

        [Test]
        public void StatParse_EdgeTrims()
        {
            AssertStatParse("\"250 Name 1.23m\"", 250, "Name", "1.23m");
            AssertStatParse("\"250 Name 1.23m", 250, "Name", "1.23m");
            AssertStatParse("250 Name 1.23m\"", 250, "Name", "1.23m");
        }

        [Test]
        public void StatParse_AdditionalInfo()
        {
            AssertStatParse("DL 245 BossHogg 3.7", 245, "DL BossHogg", "3.7");
            AssertStatParse("DL 245 BossHogg 3.7 duck!", 245, "DL BossHogg", "3.7", "duck!");
            AssertStatParse("245 BossHogg 3.7 super duck dawg!", 245, "BossHogg", "3.7", "super duck dawg!");
        }

        [Test]
        public void StatParse_NamesWithNumbers()
        {
            AssertStatParse("250 Tical2000 1.23m", 250, "Tical2000", "1.23m");
            AssertStatParse("250 Tical 2000 1.23m", 250, "Tical 2000", "1.23m");
        }

        private void AssertStatParse(string input, int? level, string name, string defense, string additionalInfo = null)
        {
            var expected = new Stat
            {
                Level = level,
                Name = name,
                Defense = defense,
                AdditionalInfo = additionalInfo
            };

            Assert.AreEqual(expected, Stat.Parse(input));
        }
    }
}
