using System;

namespace PlanetsideSeaState.Graphing.Models.Events
{
    public class FacilityOwnerUpdate
    {
        public int FacilityId { get; private set; }
        public int FactionId { get; private set; }
        public DateTime Timestamp { get; private set; }

        public FacilityOwnerUpdate(int facilityId, int factionId, DateTime timestamp)
        {
            FacilityId = facilityId;
            FactionId = factionId;
            Timestamp = timestamp;
        }
    }
}
