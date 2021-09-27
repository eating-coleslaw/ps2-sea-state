using PlanetsideSeaState.Data.Models.Census;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Graphing.Models.Nodes
{
    public class PlayerNode
    {
        public string Id { get; }
        public string Name { get; }
        public int FactionId { get; }
        public DateTime LastSeen { get; private set; }
        public int ZoneId { get; private set; }

        public TimeSpan TimeSinceLastUpdate => DateTime.UtcNow - LastSeen;

        public PlayerNode(Character character, DateTime timestamp, int zoneId)
        {
            Id = character.Id;
            Name = character.Name;
            FactionId = character.FactionId;
            LastSeen = timestamp;
            ZoneId = zoneId;
        }

        public void UpdateLocation(int zoneId, DateTime timestamp)
        {
            ZoneId = zoneId;
            LastSeen = timestamp;
        }
    }
}
