using System.ComponentModel.DataAnnotations;

namespace PlanetsideSeaState.Data.Models.Census
{
    public class VehicleFaction
    {
        [Required]
        public int VehicleId { get; set; }

        [Required]
        public int FactionId { get; set; }

        public Vehicle Vehicle { get; set; }
    }
}
