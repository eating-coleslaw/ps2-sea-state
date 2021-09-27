﻿using System.ComponentModel.DataAnnotations;

namespace PlanetsideSeaState.Data.Models.Census
{
    public class Item
    {
        [Required]
        public int Id { get; set; }

        public int? ItemTypeId { get; set; }
        public int? ItemCategoryId { get; set; }
        public bool IsVehicleWeapon { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? FactionId { get; set; }
        public int? MaxStackSize { get; set; }
        public int? ImageId { get; set; }

        public ItemCategory ItemCategory { get; set; }
    }
}
