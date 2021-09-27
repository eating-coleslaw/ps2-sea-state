using PlanetsideSeaState.Data.Models.Census;
using PlanetsideSeaState.Graphing.Models;
using PlanetsideSeaState.Graphing.Models.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Graphing.Tests
{
    public static class FacilityGraphHelper
    {
        /*
         * Mock Facility Map:
         * 
         *         D          
         *         |          
         *         |          
         *         C----E----G
         *         | \  |     
         *   A-----B  \ |     
         *             `F     
         * 
         */

        public class CompleteMapGraphFixture : IDisposable
        {
            public FacilityGraph Graph { get; private set; }
            private static readonly DateTime _seedTime = new(2021, 5, 1, 12, 0, 0);

            public CompleteMapGraphFixture()
            {
                Graph = new();

                Graph.AddNode(GetFacilityNodeA(_seedTime, 1));
                Graph.AddNode(GetFacilityNodeB(_seedTime, 1));
                Graph.AddNode(GetFacilityNodeC(_seedTime, 1));
                Graph.AddNode(GetFacilityNodeD(_seedTime, 2));
                Graph.AddNode(GetFacilityNodeF(_seedTime, 2));
                Graph.AddNode(GetFacilityNodeE(_seedTime, 3));
                Graph.AddNode(GetFacilityNodeG(_seedTime, 3));

                Graph.AddConnection(1, 2); // A-B
                Graph.AddConnection(2, 3); // B-C
                Graph.AddConnection(3, 4); // C-D
                Graph.AddConnection(3, 5); // C-E
                Graph.AddConnection(3, 6); // C-F
                Graph.AddConnection(5, 6); // E-F
                Graph.AddConnection(5, 7); // E-G

                Graph.MarkMapComplete();
            }

            public void Dispose()
            {
                Graph.Dispose();
            }
        }

        public static FacilityNode GetFacilityNodeA(DateTime seedTime, int owningFactionId) => new(GetMapRegionA(), owningFactionId, seedTime);
        public static FacilityNode GetFacilityNodeB(DateTime seedTime, int owningFactionId) => new(GetMapRegionB(), owningFactionId, seedTime);
        public static FacilityNode GetFacilityNodeC(DateTime seedTime, int owningFactionId) => new(GetMapRegionC(), owningFactionId, seedTime);
        public static FacilityNode GetFacilityNodeD(DateTime seedTime, int owningFactionId) => new(GetMapRegionD(), owningFactionId, seedTime);
        public static FacilityNode GetFacilityNodeE(DateTime seedTime, int owningFactionId) => new(GetMapRegionE(), owningFactionId, seedTime);
        public static FacilityNode GetFacilityNodeF(DateTime seedTime, int owningFactionId) => new(GetMapRegionF(), owningFactionId, seedTime);
        public static FacilityNode GetFacilityNodeG(DateTime seedTime, int owningFactionId) => new(GetMapRegionG(), owningFactionId, seedTime);

        #region Map Regions
        public static MapRegion GetMapRegionA()
        {
            return new()
            {
                Id = 1,
                FacilityId = 1,
                FacilityName = "WarpgateA",
                FacilityTypeId = 7, // Warpgate
                ZoneId = 2          // Indar
            };
        }

        public static MapRegion GetMapRegionB()
        {
            return new()
            {
                Id = 2,
                FacilityId = 2,
                FacilityName = "SmallOutpostB",
                FacilityTypeId = 6, // Small Outpost
                ZoneId = 2          // Indar
            };
        }

        public static MapRegion GetMapRegionC()
        {
            return new()
            {
                Id = 3,
                FacilityId = 3,
                FacilityName = "LargeOutpostC",
                FacilityTypeId = 5, // Large Outpost
                ZoneId = 2          // Indar
            };
        }

        public static MapRegion GetMapRegionD()
        {
            return new()
            {
                Id = 4,
                FacilityId = 4,
                FacilityName = "WarpgateD",
                FacilityTypeId = 7, // Warpgate
                ZoneId = 2          // Indar
            };
        }

        public static MapRegion GetMapRegionE()
        {
            return new()
            {
                Id = 5,
                FacilityId = 5,
                FacilityName = "SmallOutpostE",
                FacilityTypeId = 6, // Small Outpost
                ZoneId = 2          // Indar
            };
        }

        public static MapRegion GetMapRegionF()
        {
            return new()
            {
                Id = 6,
                FacilityId = 6,
                FacilityName = "SmallOutpostF",
                FacilityTypeId = 6, // Small Outpost
                ZoneId = 2          // Indar
            };
        }

        public static MapRegion GetMapRegionG()
        {
            return new()
            {
                Id = 7,
                FacilityId = 7,
                FacilityName = "WarpgateG",
                FacilityTypeId = 7, // Warpgate
                ZoneId = 2          // Indar
            };
        }
        #endregion Map Regions


    }
}
