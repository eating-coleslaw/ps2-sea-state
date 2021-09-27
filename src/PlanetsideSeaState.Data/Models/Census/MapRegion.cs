﻿using System.ComponentModel.DataAnnotations;

namespace PlanetsideSeaState.Data.Models.Census
{
    public class MapRegion
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int FacilityId { get; set; }

        public string FacilityName { get; set; }
        public int FacilityTypeId { get; set; }
        public string FacilityType { get; set; }
        public int ZoneId { get; set; }
        public bool IsDeprecated { get; set; }
        public bool IsCurrent { get; set; }
    }
}
