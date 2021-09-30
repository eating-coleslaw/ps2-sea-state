using PlanetsideSeaState.App.CensusStream.Models;
using PlanetsideSeaState.Data.Models.Census;
using System;

namespace PlanetsideSeaState.Graphing.Models.Events
{
    public class FacilityRelationEvent
    {
        public Character Character { get; set; }
        public string PlayerId { get; set; }
        public int FacilityId { get; set; }
        public DateTime Timestamp { get; set; }
        public uint ZoneId { get; set; }
        public PayloadEventType EventType { get; }
    }
}
