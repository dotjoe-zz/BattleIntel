using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupMe;

namespace BattleIntel.Core.Tests
{
    [TestFixture]
    class EpochExtensions_Fixture
    {
        [Test]
        public void Epoch_ConvertToEpoch()
        {
            Assert.AreEqual(1399294564, new DateTime(2014, 5, 5, 7, 56, 4, DateTimeKind.Local).ToEpoch());
            Assert.AreEqual(1399276564, new DateTime(2014, 5, 5, 7, 56, 4, DateTimeKind.Utc).ToEpoch());
        }

        [Test]
        public void Epoch_ConvertFromEpoch()
        {
            Assert.AreEqual(new DateTime(2014, 5, 5, 7, 56, 4, DateTimeKind.Local), (1399294564).ToDateTime(DateTimeKind.Local));
            Assert.AreEqual(new DateTime(2014, 5, 5, 7, 56, 4, DateTimeKind.Utc), (1399276564).ToDateTime(DateTimeKind.Utc));
        }
    }
}
