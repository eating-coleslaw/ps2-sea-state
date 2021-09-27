using System.ComponentModel.DataAnnotations;

namespace PlanetsideSeaState.Data.Models.Census
{
    public class MetagameEventState
    {
        [Required]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
