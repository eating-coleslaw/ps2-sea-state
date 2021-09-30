using System;

namespace PlanetsideSeaState.App.CensusStream.Models
{
    public class PayloadBase
    {
        public string EventName { get; set; }
        public short WorldId { get; set; }
        public uint? ZoneId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
