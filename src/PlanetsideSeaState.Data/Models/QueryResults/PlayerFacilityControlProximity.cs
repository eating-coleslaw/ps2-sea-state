using System;

namespace PlanetsideSeaState.Data.Models.QueryResults
{
    public class PlayerFacilityControlProximity
    {
        public string CharacterId { get; set; }
        public int FacilityId { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid FacilityControlId { get; set; }
        public short NewFactionId { get; set; }
        public uint ZoneId { get; set; }

        public TimeSpan TimeDiff { get; set; }
        public short TimeDiffDirection { get; set; }
        public double TimeDiffSeconds => TimeDiff.TotalSeconds * TimeDiffDirection;
    }
}
