using PlanetsideSeaState.Data.Models.Census;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetsideSeaState.MapActivityGraph
{
    public class PlayerNode
    {
        public readonly string CharacterId;
        public readonly int FactionId;

        public int WorldId { get; set; }

        private List<PlayerEdge> Edges { get; set; } = new();
        public IReadOnlyList<PlayerEdge> ReadOnlyEdges { get => Edges; }

        public int EdgesCount { get => Edges.Count; }

        public PlayerNode(Character character, int worldId)
        {
            CharacterId = character.Id;
            FactionId = character.FactionId;
            WorldId = worldId;
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
