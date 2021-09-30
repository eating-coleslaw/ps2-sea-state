using System;
using System.ComponentModel.DataAnnotations;

namespace PlanetsideSeaState.Data.Models.Events
{
    public class ContinentUnlock
    {
        [Required]
        public short WorldId { get; set; }
        [Required]
        public uint ZoneId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        public int? MetagameEventId { get; set; }
        public int? TriggeringFaction { get; set; }
    }
}
