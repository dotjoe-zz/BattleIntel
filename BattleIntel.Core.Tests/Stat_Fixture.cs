﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleIntel.Core.Tests
{
    [TestFixture]
    public class Stat_Fixture
    {
        [Test]
        public void StatParse_Standard()
        {
            AssertStatParse("Name 250 1.23m", 250, "Name", "1.23m", 1230000);
            AssertStatParse("250 Name 1.23m", 250, "Name", "1.23m", 1230000);
            AssertStatParse("250 1.23m Name", 250, "Name", "1.23m", 1230000);
        }

        [Test]
        public void StatParse_MissingPieces()
        {
            AssertStatParse("250 1.23m", 250, null, "1.23m", 1230000);
            AssertStatParse("250 Name", 250, "Name", null, null);
            AssertStatParse("Name 1.23m", null, "Name", "1.23m", 1230000);
        }

        [Test]
        public void StatParse_JunkIndicators()
        {
            AssertStatParse("Name lvl 250 def 1.23", 250, "Name", "1.23", 1230000);
            AssertStatParse("Name lv 250 d 1.23", 250, "Name", "1.23", 1230000);
            AssertStatParse("Name l250 d1.23", 250, "Name", "1.23", 1230000);
            AssertStatParse("Name L250 D1.23", 250, "Name", "1.23", 1230000);
            AssertStatParse("Name Lvl250 Def1.23", 250, "Name", "1.23", 1230000);
            AssertStatParse("Name Lv250 def1.23", 250, "Name", "1.23", 1230000);
        }

        [Test]
        public void StatParse_Canadian()
        {
            AssertStatParse("Name 250 1,2m", 250, "Name", "1.2m", 1200000);
            AssertStatParse("Name 250 1,23m", 250, "Name", "1.23m", 1230000);
            AssertStatParse("Name 250 1,23M", 250, "Name", "1.23M", 1230000);

            AssertStatParse("250 1,2m Name", 250, "Name", "1.2m", 1200000);
            AssertStatParse("250 1,23m Name", 250, "Name", "1.23m", 1230000);
            AssertStatParse("250 1,23M Name", 250, "Name", "1.23M", 1230000);

            AssertStatParse("Name 250 1,234k", 250, "Name", "1234k", 1234000); //not really Canadian, we just removed the thousandth's separator
        }

        [Test]
        public void StatParse_Compressed()
        {
            AssertStatParse("Name/250/1.23m", 250, "Name", "1.23m", 1230000);
            AssertStatParse("250,1.23m,Name", 250, "Name", "1.23m", 1230000);
            AssertStatParse("250-1.23m-Name", 250, "Name", "1.23m", 1230000);
            AssertStatParse("250-1,23m-Name", 250, "Name", "1.23m", 1230000); //compressed Canadian
        }

        [Test]
        public void StatParse_EdgeTrims()
        {
            AssertStatParse("\"250 Name 1.23m\"", 250, "Name", "1.23m", 1230000);
            AssertStatParse("\"250 Name 1.23m", 250, "Name", "1.23m", 1230000);
            AssertStatParse("250 Name 1.23m\"", 250, "Name", "1.23m", 1230000);
        }

        [Test]
        public void StatParse_AdditionalInfo()
        {
            AssertStatParse("DL 245 BossHogg 3.7", 245, "DL BossHogg", "3.7", 3700000);
            AssertStatParse("DL 245 BossHogg 3.7 duck!", 245, "DL BossHogg", "3.7", 3700000, "duck!");
            AssertStatParse("245 BossHogg 3.7 super duck dawg!", 245, "BossHogg", "3.7", 3700000, "super duck dawg!");

            //Legends style
            AssertStatParse("Dragon Slayer 120 200k (52k)", 120, "Dragon Slayer", "200k", 200000, "(52k)");
            AssertStatParse("Dragon Slayer 120 (52k) 200k ", 120, "Dragon Slayer (52k)", "200k", 200000); //number in parens is not matched for def
        }

        [Test]
        public void StatParse_NamesWithNumbers()
        {
            AssertStatParse("250 Tical2000 1.23m", 250, "Tical2000", "1.23m", 1230000);
            AssertStatParse("250 Tical2.0 1.23m", 250, "Tical2.0", "1.23m", 1230000);

            AssertStatParse("250 Tical 2000 1.23m", 250, "Tical 2000", "1.23m", 1230000); //picks defense number with m indicator
            AssertStatParse("250 Tical 2.0 1.23m", 250, "Tical 2.0", "1.23m", 1230000); //picks defense number with m indicator
            AssertStatParse("250 Tical 2.0 1234k", 250, "Tical 2.0", "1234k", 1234000); //picks defense number with k indicator

            AssertStatParse("250 Tical 1.23m 2.0", 250, "Tical", "1.23m", 1230000, "2.0"); //uses first number due to m indicator
            AssertStatParse("250 Tical 1.23 2.0", 250, "Tical", "1.23", 1230000, "2.0"); //uses first number by default

            AssertStatParse("136 2nutty4u 7.3", 136, "2nutty4u", "7.3", 7300000);
        }

        [Test]
        public void StatParse_DefenseValueEdgeCaseRecognition()
        {
            AssertStatParse("250 Name 40", 250, "Name", "40", 40000); //assume k
            AssertStatParse("250 Name 39", 250, "Name", "39", 39000000); //assume m
            AssertStatParse("250 Name 2", 250, "Name", "2", 2000000); //assume m

            AssertStatParse("250 Name 10000", 250, "Name", "10000", 10000); 
            AssertStatParse("250 Name 9999", 250, "Name", "9999", 9999000); //assume k
            AssertStatParse("250 Name 999", 250, "Name", "999", 999000); //assume k
            
        }

        private void AssertStatParse(string input, int? level, string name, string defense, decimal? defenseValue, string additionalInfo = null)
        {
            var expected = new Stat
            {
                Level = level,
                Name = name,
                Defense = defense,
                DefenseValue = defenseValue,
                AdditionalInfo = additionalInfo
            };

            Assert.AreEqual(expected, Stat.Parse(input));
        }
    }
}
