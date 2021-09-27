using PlanetsideSeaState.Shared.Planetside;
using PlanetsideSeaState.Data.Models.Census;
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

        public int? OldFactionId { get; set; }
        public int? NewFactionId { get; set; }

        public int? ZoneId { get; set; }
        public int WorldId { get; set; }

        public int Points { get; set; }

        #region Navigation Properties
        public Zone Zone { get; set; }
        public World World { get; set; }
        #endregion Navigation Properties
    }
}
