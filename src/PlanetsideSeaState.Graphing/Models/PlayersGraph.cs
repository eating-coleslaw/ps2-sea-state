using PlanetsideSeaState.Graphing.Exceptions;
using PlanetsideSeaState.Graphing.Models.Events;
using PlanetsideSeaState.Graphing.Models.Relations;
using PlanetsideSeaState.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Graphing.Models
{
    public class PlayersGraph
    {
        /// <summary>
        ///     ConcurrentDictionary mapping PlayerNodes to a HashSet
        ///     of the PlayerNodes to which they're connected
        /// </summary>
        private ConcurrentDictionary<PlayerNode, HashSet<PlayerNode>> PlayerConnections { get; set; } = new();

        /// <summary>
        ///     ConcurrentDictionary mapping two Player IDs to the PlayerRelation
        ///     detailing when and how the two players last met
        /// </summary>
        private ConcurrentDictionary<(string primaryPlayerId, string secondaryPlayerId), PlayerRelation> Relations { get; set; } = new();

        public int PlayerCount { get; private set; } = 0;
        public int EdgeCount { get; private set; } = 0;

        private readonly KeyedSemaphoreSlim _keyedSemaphore = new();


        public async Task AddNode(PlayerNode playerNode)
        {
            using (await _keyedSemaphore.WaitAsync($"{playerNode.Id}"))
            {
                if (!PlayerConnections.ContainsKey(playerNode))
                {
                    throw new DuplicateGraphNodeException($"{playerNode.Name} is already in the players graph");
                }

                var playersSet = new HashSet<PlayerNode>();

                if (PlayerConnections.TryAdd(playerNode, playersSet))
                {
                    PlayerCount++;
                }
            }
        }

        public async Task RemoveNode(PlayerNode playerNode)
        {
            using (await _keyedSemaphore.WaitAsync($"{playerNode.Id}"))
            {
                var playerId = playerNode.Id;

                if (PlayerConnections.TryRemove(playerNode, out var _))
                {
                    PlayerCount--;
                }
                else
                {
                    // TODO: throw error?
                    return;
                }
            }

            var neighbors = await GetNeighborsAsync(playerNode);

            if (neighbors == null)
            {
                // TODO: throw error?
                return;
            }

            foreach (var neighbor in neighbors)
            {
                await RemoveRelationAsync(neighbor, playerNode);
                await RemoveRelationAsync(playerNode, neighbor);
            }
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

        private async Task AddOrUpdateSingleRelationInternal(PlayerNode keyPlayerNode, PlayerNode otherPlayerNode, PlayerRelationEvent relationEvent)
        {
            if (await GetRelationAsync(keyPlayerNode, otherPlayerNode) != null)
            {
                await UpdateRelationAsync(keyPlayerNode, otherPlayerNode, relationEvent);
            }
            else
            {
                var relation = new PlayerRelation(otherPlayerNode, relationEvent.Timestamp, relationEvent.EventType, relationEvent.ZoneId.Value, relationEvent.ExperienceId);

                await AddRelationAsync(keyPlayerNode, otherPlayerNode, relation);
            }
        }


        private async Task AddRelationAsync(PlayerNode lhsPlayerNode, PlayerNode rhsPlayerNode, PlayerRelation relation)
        {
            PlayerConnections.TryGetValue(lhsPlayerNode, out var lhsConnections);
            PlayerConnections.TryGetValue(rhsPlayerNode, out var rhsConnections);

            if (lhsConnections == null || rhsConnections == null)
            {
                throw new KeyNotFoundException();
            }

            using (await _keyedSemaphore.WaitAsync($"{lhsPlayerNode.Id}"))
            {
                if (!lhsConnections.Add(rhsPlayerNode))
                {
                    throw new DuplicateGraphNodeException();
                }
            }

            using (await _keyedSemaphore.WaitAsync($"{rhsPlayerNode.Id}"))
            {
                if (!rhsConnections.Add(lhsPlayerNode))
                {
                    throw new DuplicateGraphNodeException();
                }
            }

            using (await _keyedSemaphore.WaitAsync($"{lhsPlayerNode.Id}^{rhsPlayerNode.Id}"))
            {
                if (Relations.TryAdd((lhsPlayerNode.Id, rhsPlayerNode.Id), relation))
                {
                    EdgeCount++;
                }
            }
        }

        private async Task UpdateRelationAsync(PlayerNode lhsPlayerNode, PlayerNode rhsPlayerNode, PlayerRelationEvent update)
        {
            using (await _keyedSemaphore.WaitAsync($"{lhsPlayerNode.Id}^{rhsPlayerNode.Id}"))
            {
                if (!Relations.TryGetValue((lhsPlayerNode.Id, rhsPlayerNode.Id), out var relation))
                {
                    throw new KeyNotFoundException();
                }

                relation.TryUpdate(update.Timestamp, update.EventType, update.ZoneId.Value, update.ExperienceId);
            }
        }

        public async Task RemoveRelationAsync(PlayerNode lhsPlayerNode, PlayerNode rhsPlayerNode)
        {
            PlayerConnections.TryGetValue(lhsPlayerNode, out var lhsNeighbors);
            PlayerConnections.TryGetValue(rhsPlayerNode, out var rhsNeighbors);

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
                if (PlayerConnections.TryGetValue(playerNode, out var neighbors))
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

        public async Task<IEnumerable<PlayerRelation>> GetRelationsAsync(PlayerNode playerNode)
        {
            var neighbors = await GetNeighborsAsync(playerNode);

            if (neighbors == null)
            {
                return null;
            }

            var relationTasks = new List<Task<PlayerRelation>>();

            foreach (var neighbor in neighbors)
            {
                relationTasks.Add(GetRelationAsync(playerNode, neighbor));
            }

            var relations = new List<PlayerRelation>();

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

        public async Task<PlayerRelation> GetRelationAsync(PlayerNode lhsPlayerNode, PlayerNode rhsPlayerNode)
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
