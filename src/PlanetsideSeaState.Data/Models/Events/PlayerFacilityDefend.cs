using PlanetsideSeaState.Data.Models.Census;
using System;
using System.ComponentModel.DataAnnotations;

namespace PlanetsideSeaState.Data.Models.Events
{
    public class PlayerFacilityDefend
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public int FacilityId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        public Guid FacilityControlId { get; set; }

        public short WorldId { get; set; }
        public uint ZoneId { get; set; }
        public string OutfitId { get; set; }

        #region Navigation Properties
        public Character Character { get; set; }
        public MapRegion Facility { get; set; }
        public FacilityControl FacilityControl { get; set; }
        #endregion Navigation Properties
    }
}
