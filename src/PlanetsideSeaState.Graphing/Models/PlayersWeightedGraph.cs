using PlanetsideSeaState.Data.Models.QueryResults;
using PlanetsideSeaState.Graphing.Exceptions;
using PlanetsideSeaState.Graphing.Models.Events;
using PlanetsideSeaState.Graphing.Models.Nodes;
using PlanetsideSeaState.Shared;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Graphing.Models
{
    public class PlayersWeightedGraph //<PlayerNode, PlayerEdge>
    {

        /// <summary>
        ///     ConcurrentDictionary mapping PlayerNodes to a HashSet of the PlayerNodes to which they're connected
        /// </summary>
        private ConcurrentDictionary<PlayerNode, HashSet<PlayerEdge>> PlayerConnections { get; set; } = new();

        public IReadOnlyDictionary<PlayerNode, HashSet<PlayerEdge>> ReadOnlyPlayerConnections => PlayerConnections;

        private ConcurrentDictionary<string, PlayerNode> PlayerNodesMap { get; set; } = new();

        public int PlayerCount { get; private set; }
        public int EdgeCount { get; private set; }

        private readonly KeyedSemaphoreSlim _keyedSemaphore = new();

        public PlayersWeightedGraph()
        {
        }

        public PlayersWeightedGraph(params PlayerNode[] playerNodes)
        {
            foreach (var node in playerNodes)
            {
                AddNode(node);
            }
        }
        
        public PlayersWeightedGraph(IEnumerable<PlayerNode> playerNodes, IEnumerable<PlayerRelationEvent> relationEvents)
        {
            foreach (var node in playerNodes)
            {
                AddNode(node);
            }

            foreach (var relationEvent in relationEvents)
            {
                var lhsNode = GetPlayerNode(relationEvent.ActingCharacter.Id);
                var rhsNode = GetPlayerNode(relationEvent.RecipientCharacter.Id);

                if (lhsNode != null && rhsNode != null)
                {
                    AddOrUpdateRelation(lhsNode, rhsNode, relationEvent);
                }
            }
        }

        public PlayersWeightedGraph(IEnumerable<PlayerNode> playerNodes, IEnumerable<PlayerConnectionEvent> connectionEvents)
        {
            foreach (var node in playerNodes)
            {
                AddNode(node);
            }

            foreach (var connectionEvent in connectionEvents)
            {
                var lhsNode = GetPlayerNode(connectionEvent.ActingCharacterId);
                var rhsNode = GetPlayerNode(connectionEvent.RecipientCharacterId);

                if (lhsNode != null && rhsNode != null)
                {
                    AddOrUpdateRelation(lhsNode, rhsNode, connectionEvent);
                }
            }
        }

        public bool AddNode(PlayerNode playerNode)
        {
            if (PlayerConnections.ContainsKey(playerNode) || PlayerNodesMap.ContainsKey(playerNode.Id))
            {
                return false;
            }
            
            var edgesSet = new HashSet<PlayerEdge>();
            PlayerConnections.TryAdd(playerNode, edgesSet);

            PlayerNodesMap.TryAdd(playerNode.Id, playerNode);

            PlayerCount++;
            
            return true;
        }

        public async Task<bool> AddNodeAsync(PlayerNode playerNode)
        {
            using (await _keyedSemaphore.WaitAsync($"{playerNode.Id}"))
            {
                var edgesSet = new HashSet<PlayerEdge>();
                if (PlayerConnections.TryAdd(playerNode, edgesSet))
                {
                    PlayerNodesMap[playerNode.Id] = playerNode;

                    PlayerCount++;
                    return true;
                }

                return false;
            }
        }

        public async Task RemoveNodeAsync(PlayerNode playerNode)
        {
            using (await _keyedSemaphore.WaitAsync($"{playerNode.Id}"))
            {
                var playerId = playerNode.Id;

                if (!PlayerConnections.ContainsKey(playerNode))
                {
                    return;
                }

                if (PlayerConnections.TryRemove(playerNode, out var _))
                {
                    PlayerNodesMap.TryRemove(playerNode.Id, out var _);

                    PlayerCount--;
                }
                else
                {
                    // TODO: throw error?
                    return;
                }
            }

            // Remove this node from all other nodes it was connected to
            foreach (var connection in playerNode.ReadOnlyEdges)
            {
                using (await _keyedSemaphore.WaitAsync($"{connection.Child.Id}"))
                {
                    if (connection.Child.RemovePlayerConnection(playerNode, out var edge))
                    {
                        PlayerConnections.TryGetValue(connection.Child, out var childConnections);

                        childConnections?.Remove(edge);
                    }

                    EdgeCount--;
                }
            }
        }

        #region PlayerConnectionEvent
        public void AddOrUpdateRelation(PlayerNode lhsPlayerNode, PlayerNode rhsPlayerNode, PlayerConnectionEvent connectionEvent)
        {
            AddOrUpdateSingleRelationInternal(lhsPlayerNode, rhsPlayerNode, connectionEvent);
            AddOrUpdateSingleRelationInternal(rhsPlayerNode, lhsPlayerNode, connectionEvent);
        }

        private void AddOrUpdateSingleRelationInternal(PlayerNode parent, PlayerNode child, PlayerConnectionEvent connectionEvent)
        {
            if (parent.TryGetConnection(child, out PlayerEdge connection))
            {
                connection.TryUpdate(connectionEvent);
            }
            else
            {
                connection = new PlayerEdge(parent, child, connectionEvent);

                connection.ExpirationReached += async (s, e) => await OnConnectionExpiredEvent(s, e);

                parent.AddEdge(connection);

                PlayerConnections.TryGetValue(parent, out HashSet<PlayerEdge> connectionSet);
                connectionSet?.Add(connection);
            }
        }

        public async Task AddOrUpdateRelationAsync(PlayerNode lhsPlayerNode, PlayerNode rhsPlayerNode, PlayerConnectionEvent connectionEvent)
        {
            await Task.WhenAll(
                AddOrUpdateSingleRelationInternalAsync(lhsPlayerNode, rhsPlayerNode, connectionEvent),
                AddOrUpdateSingleRelationInternalAsync(rhsPlayerNode, lhsPlayerNode, connectionEvent)
            );
        }

        private async Task AddOrUpdateSingleRelationInternalAsync(PlayerNode parent, PlayerNode child, PlayerConnectionEvent connectionEvent)
        {
            using (await _keyedSemaphore.WaitAsync($"{parent.Id}"))
            {
                if (parent.TryGetConnection(child, out PlayerEdge connection))
                {
                    connection.TryUpdate(connectionEvent);
                }
                else
                {
                    connection = new PlayerEdge(parent, child, connectionEvent);

                    connection.ExpirationReached += async (s, e) => await OnConnectionExpiredEvent(s, e);

                    parent.AddEdge(connection);

                    PlayerConnections.TryGetValue(parent, out HashSet<PlayerEdge> connectionSet);
                    connectionSet?.Add(connection);
                }
            }
        }
        #endregion PlayerConnectionEvent

        #region PlayerRelationEvent
        public void AddOrUpdateRelation(PlayerNode lhsPlayerNode, PlayerNode rhsPlayerNode, PlayerRelationEvent relationEvent)
        {
            if (!relationEvent.ZoneId.HasValue)
            {
                return;
            }

            AddOrUpdateSingleRelationInternal(lhsPlayerNode, rhsPlayerNode, relationEvent);
            AddOrUpdateSingleRelationInternal(rhsPlayerNode, lhsPlayerNode, relationEvent);
        }

        private void AddOrUpdateSingleRelationInternal(PlayerNode parent, PlayerNode child, PlayerRelationEvent relationEvent)
        {
            if (parent.TryGetConnection(child, out PlayerEdge connection))
            {
                connection.TryUpdate(relationEvent);
            }
            else
            {
                connection = new PlayerEdge(parent, child, relationEvent);

                connection.ExpirationReached += async (s, e) => await OnConnectionExpiredEvent(s, e);

                parent.AddEdge(connection);

                PlayerConnections.TryGetValue(parent, out HashSet<PlayerEdge> connectionSet);
                connectionSet?.Add(connection);
            }
        }

        public async Task AddOrUpdateRelationAsync(PlayerNode lhsPlayerNode, PlayerNode rhsPlayerNode, PlayerRelationEvent relationEvent)
        {
            if (!relationEvent.ZoneId.HasValue)
            {
                // TODO: throw new error
                return;
            }

            await Task.WhenAll(
                AddOrUpdateSingleRelationInternalAsync(lhsPlayerNode, rhsPlayerNode, relationEvent),
                AddOrUpdateSingleRelationInternalAsync(rhsPlayerNode, lhsPlayerNode, relationEvent)
            );
        }

        private async Task AddOrUpdateSingleRelationInternalAsync(PlayerNode parent, PlayerNode child, PlayerRelationEvent relationEvent)
        {
            using (await _keyedSemaphore.WaitAsync($"{parent.Id}"))
            {
                if (parent.TryGetConnection(child, out PlayerEdge connection))
                {
                    connection.TryUpdate(relationEvent);
                }
                else
                {
                    connection = new PlayerEdge(parent, child, relationEvent);

                    connection.ExpirationReached += async (s, e) => await OnConnectionExpiredEvent(s, e);

                    parent.AddEdge(connection);

                    PlayerConnections.TryGetValue(parent, out HashSet<PlayerEdge> connectionSet);
                    connectionSet?.Add(connection);
                }
            }
        }
        #endregion PlayerRelationEvent

        public async Task RemoveConnectionAsync(PlayerNode lhsPlayerNode, PlayerNode rhsPlayerNode)
        {
            if (lhsPlayerNode.TryGetConnection(rhsPlayerNode, out PlayerEdge lhsConnection))
            {
                await RemoveSingleConnectionInternalAsync(lhsConnection);
            }

            if (rhsPlayerNode.TryGetConnection(lhsPlayerNode, out PlayerEdge rhsConnection))
            {
                await RemoveSingleConnectionInternalAsync(rhsConnection);
            }
        }

        private async Task RemoveSingleConnectionInternalAsync(PlayerEdge connection)
        {
            var parent = connection.Parent;
            
            using (await _keyedSemaphore.WaitAsync($"{parent.Id}"))
            {
                parent.RemoveEdge(connection); // TODO: make sure this doesn't conflict with other event handlers for EdgeConnectionExpired

                PlayerConnections.TryGetValue(parent, out HashSet<PlayerEdge> connectionSet);
                connectionSet?.Remove(connection);
            }
        }
        
        private async Task OnConnectionExpiredEvent(object sender, EdgeExpirationEventArgs<PlayerEdge> e)
        {
            await RemoveSingleConnectionInternalAsync(e.Edge);
        }


        public async Task UpdatePlayerZoneAsync(PlayerNode playerNode, PlayerZoneUpdate update)
        {
            var playerId = update.PlayerId;

            using (await _keyedSemaphore.WaitAsync(playerId))
            {
                if (playerNode == null)
                {
                    return;
                }

                var updateTimestamp = update.Timestamp;
                var lastNodeTimestamp = playerNode.LastSeen;

                if (updateTimestamp >= lastNodeTimestamp)
                {
                    playerNode.UpdateLocation(update.ZoneId, updateTimestamp);
                }
            }
        }

        public PlayerNode GetPlayerNode(string playerId)
        {
            PlayerNodesMap.TryGetValue(playerId, out PlayerNode node);
            return node;
        }

        public HashSet<PlayerEdge> GetPlayerConnectionsSnapshot(string playerId)
        {
            var node = GetPlayerNode(playerId);

            if (node == null)
            {
                return null;
            }

            return node.ConnectionsSnapshot;
        }

        public HashSet<PlayerNode> GetPlayerNeighborsSnapshot(string playerId)
        {
            var node = GetPlayerNode(playerId);

            if (node == null)
            {
                return null;
            }

            return node.NeighborsSnapshot;
        }
    }
}
