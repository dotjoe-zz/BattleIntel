using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleIntel.Core;

namespace BattleIntel.Core.Tests
{
    [TestFixture]
    class TeamName_Fixture
    {
        [Test]
        public void TeamName_WithDate()
        {
            AssertTeamNameRemoval("Reservoir Dogs", "Reservoir Dogs 4/18");
            AssertTeamNameRemoval("SAS2", " SAS2 - 4-18-14");
            AssertTeamNameRemoval("SDC1-updated", " SDC1-updated 4/18");
            AssertTeamNameRemoval("Team K......4/18/14", " Team K......4/18/14");
            AssertTeamNameRemoval("1 SRBI", "1	19/04/2014 08:07:44	SRBI");
        }

        private void AssertTeamNameRemoval(string expected, string actual)
        {
            string[] lines = { actual, "123 duck 2m", "234 super duck 3m" };
            string parsed;
            var statLines = lines.RemoveTeamName(out parsed);

            Assert.AreEqual(expected, parsed);
            Assert.AreEqual(2, statLines.Count());
        }
    }
}
