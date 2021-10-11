using System;
using System.Collections.Generic;

namespace PlanetsideSeaState.App.Models
{
    public class FacilityControlPopulationPlayer
    {
        public string Id { get; set; }
        public short FactionId { get; set; }
        public string Name { get; set; }

        public short? TeamId { get; set; } // Faction the player is playing on (for NSO characters)

        public uint ZoneId { get; set; }
        public DateTime LastSeen { get; set; }

        public bool IsAttributed { get; set; }
        public int Visisted { get; set; }
        public int MinDistance { get; set; }
        public int MinDistanceToAtrributedFaction { get; set; }
        public int DepthsSum { get; set; }
        public FacilityControlProximityInfo ClosesFacilityControl { get; set; }
        public IDictionary<int, int> VisitedDepths { get; set; }
    }
}
