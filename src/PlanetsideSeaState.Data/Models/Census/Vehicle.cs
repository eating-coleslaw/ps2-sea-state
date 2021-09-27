using System.ComponentModel.DataAnnotations;

namespace PlanetsideSeaState.Data.Models.Census
{
    public class Vehicle
    {
        [Required]
        public int Id { get; set; }

        public string Name { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }
        public int? Cost { get; set; }
        public int? CostResourceId { get; set; }
        public int? ImageId { get; set; }
    }
}
