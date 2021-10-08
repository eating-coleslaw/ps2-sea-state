using System;

namespace PlanetsideSeaState.Data.Models.QueryResults
{
    public class PlayerFacilityControlProximity
    {
        public string CharacterId { get; set; }
        public int FacilityId { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid FacilityControlId { get; set; }
        public uint ZoneId { get; set; }
        public TimeSpan TimeDiff { get; set; }
    }
}
