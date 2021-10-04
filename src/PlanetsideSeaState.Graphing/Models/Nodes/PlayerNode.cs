using PlanetsideSeaState.Data.Models.Census;
using PlanetsideSeaState.Graphing.Models.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Graphing.Models.Nodes
{
    public class PlayerNode
    {
        public string Id { get; }
        public short FactionId { get; }
        public string Name { get; }

        public short? TeamId { get; set; } // Faction the player is playing on (for NSO characters)

        public uint ZoneId { get; set; }
        public DateTime LastSeen { get; private set; }

        private ConcurrentDictionary<PlayerNode, PlayerEdge> NeighboringRelations { get; set; } = new();

        // TODO: don't make these publicly accessible / remove them
        public ICollection<PlayerNode> Neighbors => NeighboringRelations.Keys;
        public ICollection<PlayerEdge> Connections => NeighboringRelations.Values;

        public HashSet<PlayerNode> NeighborsSnapshot { get => NeighboringRelations.Keys.ToHashSet(); }
        public HashSet<PlayerEdge> ConnectionsSnapshot { get => NeighboringRelations.Values.ToHashSet(); }

        private HashSet<PlayerEdge> Edges { get; set; } = new();
        public IReadOnlyCollection<PlayerEdge> ReadOnlyEdges { get => Edges; }
        public int EdgesCount { get; private set; }

        // TODO: add Player Expiration Timer

        public PlayerNode(string id, string name, short factionId, DateTime lastSeen, uint zoneId, short? teamId)
        {
            Id = id;
            Name = name;
            FactionId = factionId;
            LastSeen = lastSeen;
            ZoneId = zoneId;
            TeamId = teamId;
        }

        public PlayerNode(Character character, DateTime lastSeen, uint zoneId, short? teamId = null)
        {
            Id = character.Id;
            FactionId = character.FactionId;
            Name = character.Name;
            LastSeen = lastSeen;
            ZoneId = zoneId;
            TeamId = teamId;
        }

        public void UpdateLocation(PlayerZoneUpdate update)
        {
            ZoneId = update.ZoneId;
            LastSeen = update.Timestamp;
        }

        public void UpdateLocation(uint zoneId, DateTime timestamp)
        {
            ZoneId = zoneId;
            LastSeen = timestamp;
        }

        public bool AddEdge(PlayerEdge newEdge)
        {
            var childNode = newEdge.Child;

            if (NeighboringRelations.ContainsKey(childNode))
            {
                return false;
            }

            NeighboringRelations.TryAdd(childNode, newEdge);

            Edges.Add(newEdge);
            EdgesCount++;

            newEdge.ExpirationReached += RemoveEdge;

            return true;
        }

        //public void AddEdge(PlayerNode childNode, DateTime timestamp)
        //{
        //    var newEdge = new PlayerEdge(this, childNode, timestamp);

        //    newEdge.ExpirationReached += RemoveEdge;
        //}

        public bool RemovePlayerConnection(PlayerNode node, out PlayerEdge edge)
        {
            if (NeighboringRelations.TryRemove(node, out edge))
            {
                Edges.Remove(edge);
                EdgesCount--;

                return true;
            }

            return false;
        }

        public void RemoveEdge(PlayerEdge edge)
        {
            NeighboringRelations.TryRemove(edge.Child, out var _);
            Edges.Remove(edge);

            EdgesCount--;
        }

        private void RemoveEdge(object sender, EdgeExpirationEventArgs<PlayerEdge> e)
        {
            var edge = e.Edge;

            NeighboringRelations.TryRemove(edge.Child, out var _);
            Edges.Remove(edge);

            EdgesCount--;
        }

        public bool TryGetConnection(PlayerNode childNode, out PlayerEdge connection)
        {
            return NeighboringRelations.TryGetValue(childNode, out connection);
        }
    }
}
