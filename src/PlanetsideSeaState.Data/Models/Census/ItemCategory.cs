using System.ComponentModel.DataAnnotations;

namespace PlanetsideSeaState.Data.Models.Census
{
    public class ItemCategory
    {
        [Required]
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsWeaponCategory { get; set; }
    }
}
