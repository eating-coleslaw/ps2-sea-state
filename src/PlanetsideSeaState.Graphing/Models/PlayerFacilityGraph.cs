using PlanetsideSeaState.Graphing.Exceptions;
using PlanetsideSeaState.Graphing.Models.Events;
using PlanetsideSeaState.Graphing.Models.Nodes;
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
    public class PlayerFacilityGraph
    {
        /// <summary>
        ///     ConcurrentDictionary mapping FacilityNodes to a HashSet
        ///     of the PlayerNodes to which they're connected
        /// </summary>
        private ConcurrentDictionary<FacilityNode, HashSet<PlayerNode>> FacilityPlayers { get; set; } = new();

        ///// <summary>  ConcurrentDictionary mapping PlayerNodes to the FacilityNode to which they're connected </summary>
        ////public ConcurrentDictionary<PlayerNode, FacilityNode> PlayerFacilities { get; private set; } = new();
        //public ConcurrentDictionary<string, FacilityNode> PlayerFacilities { get; private set; } = new();


        /// <summary>
        ///     ConcurrentDictionary mapping PlayerNodes to the
        ///     FacilityNodes with which they've recently interacted
        /// </summary>
        //public ConcurrentDictionary<PlayerNode, FacilityNode> PlayerFacilities { get; private set; } = new();
        private ConcurrentDictionary<PlayerNode, HashSet<FacilityNode>> PlayerFacilities { get; set; } = new();
        private ConcurrentDictionary<(string playerId, int facilityId), FacilityRelation> Relations { get; set; } = new();

        public int FacilityCount { get; private set; } = 0;
        public int PlayerCount { get; private set; } = 0;
        public int EdgeCount { get; private set; } = 0;


        private readonly KeyedSemaphoreSlim _keyedSemaphore = new();


        public async Task AddNode(FacilityNode facility)
        {
            using (await _keyedSemaphore.WaitAsync($"{facility.Id}"))
            {
                if (FacilityPlayers.ContainsKey(facility))
                {
                    throw new DuplicateGraphNodeException($"The FacilityPlayers dictionary already contains {facility.Name}");
                }

                var players = new HashSet<PlayerNode>();

                if (FacilityPlayers.TryAdd(facility, players))
                {
                    FacilityCount++;
                }
            }
        }

        public async Task AddNodeAsync(PlayerNode player)
        {
            using (await _keyedSemaphore.WaitAsync($"{player.Id}"))
            {
                if (PlayerFacilities.ContainsKey(player))
                {
                    throw new DuplicateGraphNodeException($"The PlayerFacilities dictionary already contains {player.Name}");
                }

                var facilities = new HashSet<FacilityNode>();

                if (PlayerFacilities.TryAdd(player, facilities))
                {
                    PlayerCount++;
                }
            }
        }

        public async Task RemoveNode(PlayerNode player)
        {
            using (await _keyedSemaphore.WaitAsync($"{player.Id}"))
            {
                var playerId = player.Id;

                if (PlayerFacilities.TryRemove(player, out var _))
                {
                    PlayerCount--;
                }
                else
                {
                    // TODO: throw error?
                    return;
                }
            }

            var neighbors = await GetNeighborsAsync(player);

            if (neighbors == null)
            {
                // TODO: throw error?
                return;
            }

            foreach (var neighbor in neighbors)
            {
                await RemoveRelationAsync(player, neighbor);
                //Relations.TryRemove((player.Id, neighbor.Id), out var _);
            }
        }

        public async Task RemoveNode(FacilityNode facility)
        {
            using (await _keyedSemaphore.WaitAsync($"{facility.Id}"))
            {
                var facilityId = facility.Id;

                if (FacilityPlayers.TryRemove(facility, out var _))
                {
                    FacilityCount--;
                }
                else
                {
                    // TODO: throw error?
                    return;
                }
            }


            var neighbors = await GetNeighborsAsync(facility);

            if (neighbors == null)
            {
                // TODO: throw error?
                return;
            }

            foreach (var neighbor in neighbors)
            {
                await RemoveRelationAsync(neighbor, facility);
            }
        }


        public async Task AddOrUpdateRelation(PlayerNode player, FacilityNode facility, FacilityRelationEvent relationEvent)
        {
            if (await GetRelationAsync(player, facility) != null)
            {
                await UpdateRelationAsync(player, facility, relationEvent);
            }
            else
            {
                var relation = new FacilityRelation(player, facility, relationEvent.Timestamp, relationEvent.EventType, null);

                await AddRelationAsync(player, facility, relation);
            }
        }

        private async Task AddRelationAsync(PlayerNode playerNode, FacilityNode facilityNode, FacilityRelation relation)
        {
            PlayerFacilities.TryGetValue(playerNode, out var playerFacilities);
            FacilityPlayers.TryGetValue(facilityNode, out var facilityPlayers);

            if (playerFacilities == null || facilityPlayers == null)
            {
                throw new KeyNotFoundException();
            }

            using (await _keyedSemaphore.WaitAsync($"{playerNode.Id}"))
            {
                if (!playerFacilities.Add(facilityNode))
                {
                    throw new DuplicateGraphNodeException();
                }
            }

            using (await _keyedSemaphore.WaitAsync($"{facilityNode.Id}"))
            {
                if (!facilityPlayers.Add(playerNode))
                {
                    throw new DuplicateGraphNodeException();
                }
            }

            using (await _keyedSemaphore.WaitAsync($"{playerNode.Id}^{facilityNode.Id}"))
            {
                if (Relations.TryAdd((playerNode.Id, facilityNode.Id), relation))
                {
                    EdgeCount++;
                }
                else
                {
                    // TODO: throw new error
                    return;
                }
            }
        }

        private async Task UpdateRelationAsync(PlayerNode player, FacilityNode facility, FacilityRelationEvent update)
        {
            using (await _keyedSemaphore.WaitAsync($"{player.Id}^{facility.Id}"))
            {
                if (!Relations.TryGetValue((player.Id, facility.Id), out var relation))
                {
                    throw new KeyNotFoundException();
                }

                relation.TryUpdate(update.Timestamp, update.EventType, null);
            }
        }

        public async Task RemoveRelationAsync(PlayerNode player, FacilityNode facility)
        {
            PlayerFacilities.TryGetValue(player, out var playerFacilities);
            FacilityPlayers.TryGetValue(facility, out var facilityPlayers);

            if (playerFacilities == null || facilityPlayers == null)
            {
                throw new KeyNotFoundException();
            }

            using (await _keyedSemaphore.WaitAsync($"{player.Id}"))
            {
                if (!playerFacilities.Remove(facility))
                {
                    throw new KeyNotFoundException(); // TODO: no need to throw error?
                }
            }

            using (await _keyedSemaphore.WaitAsync($"{facility.Id}"))
            {
                if (!facilityPlayers.Remove(player))
                {
                    throw new KeyNotFoundException(); // TODO: no need to throw error?
                }
            }

            using (await _keyedSemaphore.WaitAsync($"{player.Id}^{facility.Id}"))
            {
                if (Relations.Remove((player.Id, facility.Id), out var _))
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
        ///     Get an array copy of a the PlayerNode connected to a single FacilityNode
        /// </summary>
        /// <param name="facility">The FacilityNode for which to get connected players</param>
        /// <returns>An array copy of the players connected to the facility</returns>
        public async Task<IEnumerable<PlayerNode>> GetNeighborsAsync(FacilityNode facility)
        {
            using (await _keyedSemaphore.WaitAsync($"{facility.Id}"))
            {
                if (FacilityPlayers.TryGetValue(facility, out var neighbors))
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

        /// <summary>
        ///     Get an array copy of a the FacilityNodes connected to a single PlayerNode
        /// </summary>
        /// <param name="player">The PlayerNode for which to get connected facilities</param>
        /// <returns>An array copy of the facilities connected to the player</returns>
        public async Task<IEnumerable<FacilityNode>> GetNeighborsAsync(PlayerNode player)
        {
            using (await _keyedSemaphore.WaitAsync($"{player.Id}"))
            {
                if (PlayerFacilities.TryGetValue(player, out var neighbors))
                {
                    var copy = new FacilityNode[neighbors.Count];

                    neighbors.CopyTo(copy);

                    return copy;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<IEnumerable<FacilityRelation>> GetRelationsAsync(PlayerNode player)
        {
            var neighbors = await GetNeighborsAsync(player);

            if (neighbors == null)
            {
                return null;
            }

            var relationTasks = new List<Task<FacilityRelation>>();

            foreach (var neighbor in neighbors)
            {
                relationTasks.Add(GetRelationAsync(player, neighbor));
            }

            var relations = new List<FacilityRelation>();

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

        public async Task<IEnumerable<FacilityRelation>> GetRelationsAsync(FacilityNode facility)
        {
            var neighbors = await GetNeighborsAsync(facility);

            if (neighbors == null)
            {
                return null;
            }

            var relationTasks = new List<Task<FacilityRelation>>();

            foreach (var neighbor in neighbors)
            {
                relationTasks.Add(GetRelationAsync(neighbor, facility));
            }

            var relations = new List<FacilityRelation>();

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

        public async Task<FacilityRelation> GetRelationAsync(PlayerNode player, FacilityNode facility)
        {
            using (await _keyedSemaphore.WaitAsync($"{player.Id}^{facility.Id}"))
            {
                if (Relations.TryGetValue((player.Id, facility.Id), out var relation))
                {
                    return relation;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
