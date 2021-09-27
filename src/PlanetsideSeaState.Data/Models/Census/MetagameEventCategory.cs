﻿using System.ComponentModel.DataAnnotations;

namespace PlanetsideSeaState.Data.Models.Census
{
    public class MetagameEventCategory
    {
        [Required]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public int? Type { get; set; }
        public int? ExperienceBonus { get; set; }
    }
}
