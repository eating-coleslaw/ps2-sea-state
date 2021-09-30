using PlanetsideSeaState.Shared.Planetside;
using System;
using System.ComponentModel.DataAnnotations;

namespace PlanetsideSeaState.Data.Models.Events
{
    public class FacilityControl
    {
        [Required]
        public DateTime Timestamp { get; set; }
        
        [Required]
        public int FacilityId { get; set; }
        
        public FacilityControlType ControlType { get; set; }

        public short? OldFactionId { get; set; }
        public short? NewFactionId { get; set; }

        public uint? ZoneId { get; set; }
        public short WorldId { get; set; }
    }
}
