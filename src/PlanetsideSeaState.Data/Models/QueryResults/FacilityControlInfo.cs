using System;

namespace PlanetsideSeaState.Data.Models.QueryResults
{
    public class FacilityControlInfo
    {
        public Guid Id { get; set; }
        public int FacilityId { get; set; }
        public short WorldId { get; set; }
        public DateTime Timestamp { get; set; }
        public string FacilityName { get; set; }

        //public FacilityControlType ControlType { get; set; }
        public bool IsCapture { get; set; }

        public short? OldFactionId { get; set; }
        public short? NewFactionId { get; set; }

        public uint? ZoneId { get; set; }

        public int MapRegionId { get; set; }
        public int FacilityTypeId { get; set; }
        public string FacilityType { get; set; }
    }
}
