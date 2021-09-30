using System;
using System.ComponentModel.DataAnnotations;

namespace PlanetsideSeaState.Data.Models.Census
{
    public class FacilityLink
    {
        [Required]
        public Guid Id { get; set; }
        
        [Required]
        public int FacilityIdA { get; set; }
        [Required]
        public int FacilityIdB { get; set; }

        [Required]
        public uint ZoneId { get; set; }

        public string Desription { get; set; }

        #region Navigation Properties
        public MapRegion FacilityA { get; set; }
        public MapRegion FacilityB { get; set; }
        #endregion Navigation Properties

    }
}
