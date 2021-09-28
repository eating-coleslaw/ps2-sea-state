using PlanetsideSeaState.Graphing.Exceptions;
using PlanetsideSeaState.Graphing.Models.Events;
using PlanetsideSeaState.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Graphing.Models
{
    public class PlayersWeightedGraph //<PlayerNode, PlayerEdge>
    {

        /// <summary>
        ///     ConcurrentDictionary mapping PlayerNodes to a HashSet of the PlayerNodes to which they're connected
        /// </summary>
        private ConcurrentDictionary<PlayerNode, HashSet<PlayerEdge>> NeighboringConnections { get; set; } = new();

        /// <summary>
        ///     ConcurrentDictionary mapping two Player IDs to the PlayerRelation detailing when and how the two players last met
        /// </summary>
        private ConcurrentDictionary<(string primaryPlayerId, string secondaryPlayerId), PlayerEdge> Relations { get; set; } = new();

        public int PlayerCount { get; private set; } = 0;
        public int EdgeCount { get; private set; } = 0;

        private readonly KeyedSemaphoreSlim _keyedSemaphore = new();


        public async Task<bool> AddNode(PlayerNode playerNode)
        {
            using (await _keyedSemaphore.WaitAsync($"{playerNode.Id}"))
            {
                var edgesSet = new HashSet<PlayerEdge>();

                if (NeighboringConnections.TryAdd(playerNode, edgesSet))
                {
                    PlayerCount++;
                    return true;
                }

                return false;
            }
        }

        public async Task RemoveNode(PlayerNode playerNode)
        {
            using (await _keyedSemaphore.WaitAsync($"{playerNode.Id}"))
            {
                var playerId = playerNode.Id;

                if (NeighboringConnections.TryRemove(playerNode, out var _))
                {
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
                        NeighboringConnections.TryGetValue(connection.Child, out var childConnections);

                        childConnections?.Remove(edge);
                    }

                    EdgeCount--;
                }
            }

            //var neighbors = await GetNeighborsAsync(playerNode);

            //if (neighbors == null)
            //{
            //    // TODO: throw error?
            //    return;
            //}

            //foreach (var neighbor in neighbors)
            //{
            //    await RemoveRelationAsync(neighbor, playerNode);
            //    await RemoveRelationAsync(playerNode, neighbor);
            //}
        }

        public async Task AddOrUpdateRelation(PlayerNode lhsPlayerNode, PlayerNode rhsPlayerNode, PlayerRelationEvent relationEvent)
        {
            if (!relationEvent.ZoneId.HasValue)
            {
                // TODO: throw new error
                return;
            }

            await Task.WhenAll(
                AddOrUpdateSingleRelationInternal(lhsPlayerNode, rhsPlayerNode, relationEvent),
                AddOrUpdateSingleRelationInternal(rhsPlayerNode, lhsPlayerNode, relationEvent)
            );
        }

        private async Task AddOrUpdateSingleRelationInternal(PlayerNode parent, PlayerNode child, PlayerRelationEvent relationEvent)
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

                    NeighboringConnections.TryGetValue(parent, out HashSet<PlayerEdge> connectionSet);
                    connectionSet?.Add(connection);
                }
            }
            
            //if (await GetRelationAsync(parent, child) != null)
            //{
            //    await UpdateRelationCoreAsync(parent, child, relationEvent);
            //}
            //else
            //{
            //    var relation = new PlayerEdge(parent, child, relationEvent);
            //    //var relation = new PlayerEdge(otherPlayerNode, relationEvent.Timestamp, relationEvent.EventType, relationEvent.ZoneId.Value, relationEvent.ExperienceId);

            //    await AddRelationCoreAsync(parent, child, relation);
            //}
        }


        //private async Task AddRelationCoreAsync(PlayerNode lhsPlayerNode, PlayerNode rhsPlayerNode, PlayerEdge relation)
        //{
        //    NeighboringConnections.TryGetValue(lhsPlayerNode, out var lhsConnections);
        //    NeighboringConnections.TryGetValue(rhsPlayerNode, out var rhsConnections);

        //    if (lhsConnections == null || rhsConnections == null)
        //    {
        //        throw new KeyNotFoundException();
        //    }

        //    using (await _keyedSemaphore.WaitAsync($"{lhsPlayerNode.Id}"))
        //    {
        //        if (!lhsConnections.Add(rhsPlayerNode))
        //        {
        //            throw new DuplicateGraphNodeException();
        //        }
        //    }

        //    using (await _keyedSemaphore.WaitAsync($"{rhsPlayerNode.Id}"))
        //    {
        //        if (!rhsConnections.Add(lhsPlayerNode))
        //        {
        //            throw new DuplicateGraphNodeException();
        //        }
        //    }

        //    using (await _keyedSemaphore.WaitAsync($"{lhsPlayerNode.Id}^{rhsPlayerNode.Id}"))
        //    {
        //        if (Relations.TryAdd((lhsPlayerNode.Id, rhsPlayerNode.Id), relation))
        //        {
        //            EdgeCount++;
        //        }
        //    }
        //}

        //private async Task UpdateRelationCoreAsync(PlayerNode lhsPlayerNode, PlayerNode rhsPlayerNode, PlayerRelationEvent update)
        //{
        //    using (await _keyedSemaphore.WaitAsync($"{lhsPlayerNode.Id}^{rhsPlayerNode.Id}"))
        //    {
        //        if (!Relations.TryGetValue((lhsPlayerNode.Id, rhsPlayerNode.Id), out var relation))
        //        {
        //            throw new KeyNotFoundException();
        //        }

        //        relation.TryUpdate(update.Timestamp, update.EventType, update.ZoneId.Value, update.ExperienceId);
        //    }
        //}

        public async Task RemoveConnectionAsync(PlayerNode lhsPlayerNode, PlayerNode rhsPlayerNode)
        {
            if (lhsPlayerNode.TryGetConnection(rhsPlayerNode, out PlayerEdge lhsConnection))
            {
                await RemoveSingleConnectionInternal(lhsConnection);
            }

            if (rhsPlayerNode.TryGetConnection(lhsPlayerNode, out PlayerEdge rhsConnection))
            {
                await RemoveSingleConnectionInternal(rhsConnection);
            }
        }

        private async Task RemoveSingleConnectionInternal(PlayerEdge connection)
        {
            var parent = connection.Parent;
            
            using (await _keyedSemaphore.WaitAsync($"{parent.Id}"))
            {
                parent.RemoveEdge(connection); // TODO: make sure this doesn't conflict with other event handlers for EdgeConnectionExpired

                NeighboringConnections.TryGetValue(parent, out HashSet<PlayerEdge> connectionSet);
                connectionSet?.Remove(connection);
            }
        }
        
        private async Task OnConnectionExpiredEvent(object sender, EdgeExpirationEventArgs<PlayerEdge> e)
        {
            await RemoveSingleConnectionInternal(e.Edge);
        }


        /*
        public async Task RemoveRelationAsync(PlayerNode lhsPlayerNode, PlayerNode rhsPlayerNode)
        {
            NeighboringConnections.TryGetValue(lhsPlayerNode, out var lhsNeighbors);
            NeighboringConnections.TryGetValue(rhsPlayerNode, out var rhsNeighbors);

            if (lhsNeighbors == null || rhsNeighbors == null)
            {
                throw new KeyNotFoundException();
            }

            using (await _keyedSemaphore.WaitAsync($"{lhsPlayerNode.Id}"))
            {
                if (!lhsNeighbors.Remove(rhsPlayerNode))
                {
                    throw new KeyNotFoundException(); // TODO: no need to throw error?
                }
            }

            using (await _keyedSemaphore.WaitAsync($"{rhsPlayerNode.Id}"))
            {
                if (!rhsNeighbors.Remove(lhsPlayerNode))
                {
                    throw new KeyNotFoundException(); // TODO: no need to throw error?
                }
            }

            using (await _keyedSemaphore.WaitAsync($"{lhsPlayerNode.Id}^{rhsPlayerNode.Id}"))
            {
                if (Relations.Remove((lhsPlayerNode.Id, rhsPlayerNode.Id), out var _))
                {
                    EdgeCount--;
                }
                else
                {
                    throw new KeyNotFoundException(); // TODO: no need to throw error?
                }
            }
        }

        /// <summary>
        ///     Get an array copy of a the PlayerNodes connected to another PlayerNode
        /// </summary>
        /// <param name="playerNode">The PlayerNode for which to get connected players</param>
        /// <returns>An array copy of the players connected to the specified player</returns>
        public async Task<IEnumerable<PlayerNode>> GetNeighborsAsync(PlayerNode playerNode)
        {
            using (await _keyedSemaphore.WaitAsync($"{playerNode.Id}"))
            {
                if (playerNode.)
                
                if (NeighboringConnections.TryGetValue(playerNode, out var neighbors))
                {
                    var copy = new PlayerNode[neighbors.Count];

                    neighbors.CopyTo(copy);

                    return copy;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<IEnumerable<PlayerEdge>> GetRelationsAsync(PlayerNode playerNode)
        {
            var neighbors = await GetNeighborsAsync(playerNode);

            if (neighbors == null)
            {
                return null;
            }

            var relationTasks = new List<Task<PlayerEdge>>();

            foreach (var neighbor in neighbors)
            {
                relationTasks.Add(GetRelationAsync(playerNode, neighbor));
            }

            var relations = new List<PlayerEdge>();

            while (relationTasks.Any())
            {
                var relationTask = await Task.WhenAny(relationTasks);

                if (relationTask.Result != null)
                {
                    relations.Add(relationTask.Result);
                }

                relationTasks.Remove(relationTask);
            }

            return relations;
        }

        public async Task<PlayerEdge> GetRelationAsync(PlayerNode lhsPlayerNode, PlayerNode rhsPlayerNode)
        {
            using (await _keyedSemaphore.WaitAsync($"{lhsPlayerNode.Id}^{rhsPlayerNode.Id}"))
            {
                if (Relations.TryGetValue((lhsPlayerNode.Id, rhsPlayerNode.Id), out var relation))
                {
                    return relation;
                }
                else
                {
                    return null;
                }
            }
        }
        */

        public async Task UpdatePlayerZone(PlayerNode playerNode, PlayerZoneUpdate update)
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


    }
}
