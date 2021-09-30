using System;

namespace PlanetsideSeaState.Graphing.Models.Events
{
    public class PlayerZoneUpdate
    {
        public string PlayerId { get; }
        public uint ZoneId { get; }
        public DateTime Timestamp { get; }

        public PlayerZoneUpdate(string playerId, uint zoneId, DateTime timestamp)
        {
            PlayerId = playerId;
            ZoneId = zoneId;
            Timestamp = timestamp;
        }
    }
}
