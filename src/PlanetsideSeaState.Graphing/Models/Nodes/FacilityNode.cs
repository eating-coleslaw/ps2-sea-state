using PlanetsideSeaState.Data.Models.Census;
using System;

namespace PlanetsideSeaState.Graphing.Models.Nodes
{
    public class FacilityNode
    {
        public int Id { get; }

        public string Name { get; }
        public int MapRegionId { get; }
        public int FacilityId { get; }
        public int FacilityTypeId { get; }

        public int OwningFactionId { get; private set; }

        public DateTime LastOwnershipTimestamp { get; private set; }

        public TimeSpan TimeSinceLastUpdate => DateTime.UtcNow - LastOwnershipTimestamp;

        public FacilityNode(MapRegion mapRegion, int owningFactionId, DateTime timestamp)
        {
            Id = mapRegion.FacilityId;

            Name = mapRegion.FacilityName;
            MapRegionId = mapRegion.Id;
            FacilityId = mapRegion.FacilityId;
            FacilityTypeId = mapRegion.FacilityTypeId;
            OwningFactionId = owningFactionId;

            LastOwnershipTimestamp = timestamp;
        }

        public void UpdateOwner(int factionId, DateTime timestamp)
        {
            OwningFactionId = factionId;
            LastOwnershipTimestamp = timestamp;
        }

        public override bool Equals(object obj)
        {
            try
            {
                var otherNode = obj as FacilityNode;

                return otherNode.FacilityId == FacilityId
                        && otherNode.MapRegionId == MapRegionId;
            }
            catch
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return $"{MapRegionId}^{FacilityId}".GetHashCode();
        }
    }
}
