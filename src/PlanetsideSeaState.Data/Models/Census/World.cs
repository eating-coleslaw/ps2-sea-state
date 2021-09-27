using System.ComponentModel.DataAnnotations;

namespace PlanetsideSeaState.Data.Models.Census
{
    public class World
    {
        [Required]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
