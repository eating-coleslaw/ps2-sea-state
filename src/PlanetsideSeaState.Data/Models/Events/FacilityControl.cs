using PlanetsideSeaState.Shared.Planetside;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlanetsideSeaState.Data.Models.Events
{
    public class FacilityControl
    {
        [Required]
        public Guid Id { get; set; }
        
        [Required]
        public DateTime Timestamp { get; set; }
        
        [Required]
        public int FacilityId { get; set; }

        public bool IsCapture { get; set; }

        //public int AttributedPlayers { get; set; }

        public short OldFactionId { get; set; }
        public short NewFactionId { get; set; }

        public uint ZoneId { get; set; }
        public short WorldId { get; set; }

        public IEnumerable<PlayerFacilityControl> PlayerControls { get; set; }
    }
}
