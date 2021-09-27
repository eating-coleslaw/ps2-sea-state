using PlanetsideSeaState.Data.Models.Census;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Graphing
{
    public class PlayerNode
    {
        public string Id { get; }
        public int FactionId { get; }
        public string Name { get; }

        public int ZoneId { get; set; }
        public DateTime LastSeen { get; private set; }

        private List<PlayerEdge> Edges { get; set; } = new();
        public IReadOnlyList<PlayerEdge> ReadOnlyEdges { get => Edges; }
        public int EdgesCount { get => Edges.Count; }


        public PlayerNode(Character character, DateTime lastSeen, int zoneId)
        {
            Id = character.Id;
            FactionId = character.FactionId;
            Name = character.Name;
            LastSeen = lastSeen;
            ZoneId = zoneId;
        }

        public void UpdateLocation(int zoneId, DateTime timestamp)
        {
            ZoneId = zoneId;
            LastSeen = timestamp;
        }

        public void AddEdge(PlayerNode childNode, DateTime timestamp)
        {
            var newEdge = new PlayerEdge(this, childNode, timestamp);

            newEdge.ExpirationReached += RemoveEdge;
        }

        public void RemoveEdge(object? sender, EdgeExpirationEventArgs<PlayerEdge> e)
        {
            Edges.Remove(e.Edge);
        }
    }
}
