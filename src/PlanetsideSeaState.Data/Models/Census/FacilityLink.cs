using System;
using System.ComponentModel.DataAnnotations;

namespace PlanetsideSeaState.Data.Models.Census
{
    public class FacilityLink
    {
        [Required]
        //public int Id { get; set; }
        public Guid Id { get; set; }
        
        [Required]
        public int FacilityIdA { get; set; }
        [Required]
        public int FacilityIdB { get; set; }

        [Required]
        public int ZoneId { get; set; }

        public string Desription { get; set; }

        #region Navigation Properties
        public MapRegion FacilityA { get; set; }
        public MapRegion FacilityB { get; set; }
        public Zone Zone { get; set; }
        #endregion Navigation Properties

    }
}
