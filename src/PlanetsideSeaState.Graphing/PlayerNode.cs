﻿using PlanetsideSeaState.Data.Models.Census;
using PlanetsideSeaState.Graphing.Models.Events;
using System;
using System.Collections.Concurrent;
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

        private ConcurrentDictionary<PlayerNode, PlayerEdge> NeighboringRelations { get; set; } = new();
        public ICollection<PlayerNode> Neighbors => NeighboringRelations.Keys;
        public ICollection<PlayerEdge> Connections => NeighboringRelations.Values;

        public HashSet<PlayerNode> NeighborsSnapshot { get => NeighboringRelations.Keys.ToHashSet(); }
        public HashSet<PlayerEdge> ConnectionsSnapshot { get => NeighboringRelations.Values.ToHashSet(); }

        private HashSet<PlayerEdge> Edges { get; set; } = new();
        public IReadOnlyCollection<PlayerEdge> ReadOnlyEdges { get => Edges; }
        public int EdgesCount { get; private set; }

        // TODO: add Player Expiration Timer


        public PlayerNode(Character character, DateTime lastSeen, int zoneId)
        {
            Id = character.Id;
            FactionId = character.FactionId;
            Name = character.Name;
            LastSeen = lastSeen;
            ZoneId = zoneId;
        }

        public void UpdateLocation(PlayerZoneUpdate update)
        {
            ZoneId = update.ZoneId;
            LastSeen = update.Timestamp;
        }

        public void UpdateLocation(int zoneId, DateTime timestamp)
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
