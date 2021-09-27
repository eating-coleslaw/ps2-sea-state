using System.ComponentModel.DataAnnotations;

namespace PlanetsideSeaState.Data.Models.Census
{
    public class Faction
    {
        [Required]
        public int Id { get; set; }

        public string Name { get; set; }
        public int? ImageId { get; set; }
        public string CodeTag { get; set; }
        public bool UserSelectable { get; set; }
    }
}
