using System.ComponentModel.DataAnnotations;

namespace PlanetsideSeaState.Data.Models.Census
{
    public class Experience
    {
        [Required]
        public int Id { get; set; }

        public string Description { get; set; }
        public float Xp { get; set; }
    }
}
