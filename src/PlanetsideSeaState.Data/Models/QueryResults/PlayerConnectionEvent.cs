using PlanetsideSeaState.Shared.Planetside;
using System;

namespace PlanetsideSeaState.Data.Models.QueryResults
{
    public class PlayerConnectionEvent
    {
        public string ActingCharacterId { get; set; }
        public string RecipientCharacterId { get; set; }
        public DateTime Timestamp { get; set; }
        public PayloadEventType EventType { get; set; }
        public short ActingFactionId { get; set; }
        public short RecipientFactionId { get; set; }
        public int? ExperienceId { get; set; }
        public uint ZoneId { get; set; }
    }
}
