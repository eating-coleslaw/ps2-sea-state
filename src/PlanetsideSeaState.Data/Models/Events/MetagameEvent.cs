using System;
using System.ComponentModel.DataAnnotations;

namespace PlanetsideSeaState.Data.Models.Events
{
    public class MetagameEvent
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
        
        [Required]
        public int WorldId { get; set; }
        
        public int? ZoneId { get; set; }
        public int? InstanceId { get; set; }
        public int? MetagameEventId { get; set; }
        public string MetagameEventState { get; set; }
        public int? ExperienceBonus { get; set; }
        public float? ZoneControlVs { get; set; }
        public float? ZoneControlNc { get; set; }
        public float? ZoneControlTr { get; set; }
    }
}
