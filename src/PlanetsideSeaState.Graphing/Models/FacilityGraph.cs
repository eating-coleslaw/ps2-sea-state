using PlanetsideSeaState.Graphing.Exceptions;
using PlanetsideSeaState.Graphing.Models.Events;
using PlanetsideSeaState.Graphing.Models.Nodes;
using PlanetsideSeaState.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Graphing.Models
{
    public class FacilityGraph : IDisposable
    {
        private HashSet<FacilityNode> Facilities { get; set; } = new();

        private Dictionary<int, FacilityNode> FacilityIdMap { get; set; } = new();
        private ConcurrentDictionary<int, HashSet<FacilityNode>> Neighbors { get; set; } = new(); // TODO: make this a normal dictionary?

        private readonly KeyedSemaphoreSlim _facilitySemaphore = new();
        private bool disposedValue;

        public bool MapIsComplete { get; private set; } = false;
        public int EdgeCount { get; private set; } = 0;
        public int FacilityCount => FacilityIdMap.Count;


        public FacilityGraph()
        {
        }

        public void AddNode(FacilityNode facilityNode)
        {
            if (MapIsComplete)
            {
                // TODO: throw new error
                return;
            }

            if (Facilities.Contains(facilityNode))
            {
                throw new DuplicateGraphNodeException();
            }

            Facilities.Add(facilityNode);
            FacilityIdMap.Add(facilityNode.Id, facilityNode);

            var neighborsSet = new HashSet<FacilityNode>();

            Neighbors.TryAdd(facilityNode.Id, neighborsSet);
        }

        public FacilityNode GetNode(int facilityId)
        {
            if (FacilityIdMap.ContainsKey(facilityId))
            {
                return FacilityIdMap[facilityId];
            }
            else
            {
                return null;
            }
        }

        public void AddConnection(int lhsId, int rhsId)
        {
            if (MapIsComplete)
            {
                // TODO: throw new error
                return;
            }

            var lhsNode = GetNode(lhsId);
            var rhsNode = GetNode(rhsId);

            if (lhsNode == null || rhsNode == null)
            {
                throw new KeyNotFoundException();
            }

            if (lhsId == rhsId)
            {
                throw new ArgumentException($"Cannot connection a facility to itself");
            }


            Neighbors.TryGetValue(lhsId, out var lhsNeighbors);
            Neighbors.TryGetValue(rhsId, out var rhsNeighbors);

            if (lhsNeighbors == null || rhsNeighbors == null)
            {
                throw new KeyNotFoundException();
            }

            if (lhsNeighbors.Add(rhsNode) && rhsNeighbors.Add(lhsNode))
            {
                //Console.WriteLine($"Added facility connection {lhsNode.Name} - {rhsNode.Name}");

                EdgeCount++;
            }

        }

        public IEnumerable<FacilityNode> GetNeighbors(FacilityNode facilityNode)
        {
            if (Neighbors.TryGetValue(facilityNode.Id, out var neighbors))
            {
                return neighbors;
            }
            else
            {
                return null;
            }
        }

        public void MarkMapComplete()
        {
            MapIsComplete = true;
        }

        public void MarkMapIncomplete()
        {
            MapIsComplete = false;
        }


        public async Task<bool> IsFacilityCapturable(int facilityId)
        {
            if (!FacilityIdMap.ContainsKey(facilityId))
            {
                //throw new ArgumentException($"facilityId is not in the map graph");
                throw new KeyNotFoundException($"facilityId is not in the map graph");
            }

            var facilityNode = FacilityIdMap[facilityId];

            // Warpgates are never capturable
            if (facilityNode.FacilityTypeId == 7)
            {
                return false;
            }

            var neighbors = GetNeighbors(facilityNode).ToList();

            using (await _facilitySemaphore.WaitAsync($"{facilityId}"))
            {
                foreach (var neighbor in neighbors)
                {
                    using (await _facilitySemaphore.WaitAsync($"{neighbor.Id}"))
                    {
                        if (neighbor.OwningFactionId != facilityNode.OwningFactionId)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public async Task UpdateFacilityOwner(FacilityOwnerUpdate update)
        {
            if (!MapIsComplete)
            {
                return;
            }

            var facilityId = update.FacilityId;

            using (await _facilitySemaphore.WaitAsync($"{facilityId}"))
            {
                var facilityNode = GetNode(facilityId);

                if (facilityNode == null)
                {
                    return;
                }

                var updateFactionId = update.FactionId;
                var updateTimestamp = update.Timestamp;
                var lastNodeTimestamp = facilityNode.LastOwnershipTimestamp;

                //facilityNode.UpdateOwner(updateFactionId, updateTimestamp);

                //if (updateTimestamp <= lastNodeTimestamp)
                if (updateTimestamp >= lastNodeTimestamp)
                {
                    facilityNode.UpdateOwner(updateFactionId, updateTimestamp);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    Facilities.Clear();
                    FacilityIdMap.Clear();
                    Neighbors.Clear();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~FacilityGraph()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
