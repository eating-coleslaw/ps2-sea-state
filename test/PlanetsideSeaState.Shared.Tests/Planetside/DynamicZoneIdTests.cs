using PlanetsideSeaState.Shared.Planetside;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PlanetsideSeaState.Shared.Tests.Planetside
{
    public class DynamicZoneIdTests
    {
        #region ZoneId Constructor
        [Theory]
        [InlineData(393577)]
        [InlineData(459113)]
        public void ZoneIdConstructor_DefinitionId_IsDesolation(uint zoneId)
        {
            var test = new DynamicZoneId(zoneId);

            Assert.Equal(361, test.DefinitionId); // 0x0169
        }

        [Fact]
        public void ZoneIdConstructor_DefinitionId_IsHossin()
        {
            var test = new DynamicZoneId(4);

            Assert.Equal(4, test.DefinitionId);
        }

        [Theory]
        [InlineData(393577, 6)]
        [InlineData(459113, 7)]
        public void ZoneIdConstructor_InstanceId_IsInstanceId(uint zoneId, ushort targetInstanceId)
        {
            var test = new DynamicZoneId(zoneId);

            Assert.Equal(targetInstanceId, test.InstanceId);
        }

        [Fact]
        public void ZoneIdConstructor_InstanceId_IsZero()
        {
            var test = new DynamicZoneId(4);

            Assert.Equal(0, test.InstanceId);
        }
        #endregion ZoneId Constructor


        #region Instance & Definition IDs Constructor
        [Fact]
        public void InstAndDefConstructor_DesolationSix_HasZoneId()
        {
            var test = new DynamicZoneId(361, 6);

            Assert.Equal(393577U, test.ZoneId);
        }

        [Fact]
        public void InstAndDefConstructor_DesolationSeven_HasZoneId()
        {
            var test = new DynamicZoneId(361, 7);

            Assert.Equal(459113U, test.ZoneId);
        }

        [Fact]
        public void InstAndDefConstructor_Hossin_HasZoneId()
        {
            var test = new DynamicZoneId(4, 0);

            Assert.Equal(4U, test.ZoneId);
        }
        #endregion Instance & Definition IDs Constructor
    }
}
