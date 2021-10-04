using Microsoft.Extensions.Logging;
//using PlanetsideSeaState.App;
//using PlanetsideSeaState.App.Services.Planetside;
using PlanetsideSeaState.CensusStore.Services;
using PlanetsideSeaState.Data.Repositories;
using PlanetsideSeaState.Graphing.Models;
using PlanetsideSeaState.Graphing.Models.Events;
using PlanetsideSeaState.Graphing.Models.Nodes;
using PlanetsideSeaState.Shared;
using PlanetsideSeaState.Shared.Planetside;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Graphing.Services.FacilityPopsGraph
{
    public class FacilityPopGraphService : StatefulService, IDisposable
    {
        private readonly IFacilityRepository _facilityRepository;
        private readonly IFacilityStore _facilityStore;
        private readonly ILogger<FacilityPopGraphService> _logger;


        private FacilityPopGraph FacilityPopGraph { get; set; } = new();

        //private FacilityGraph FacilityGraph { get; set; } = new();
        //private PlayersGraph PlayersGraph { get; set; } = new();
        //private PlayerFacilityGraph PlayerFacilityGraph { get; set; } = new();


        private bool PauseUpdates { get; set; } = true;
        private ConcurrentQueue<FacilityOwnerUpdate> FacilityOwnerQueue { get; set; } = new();
        private ConcurrentQueue<PlayerRelationEvent> PlayerRelationsQueue { get; set; } = new();
        private ConcurrentQueue<FacilityRelationEvent> FacilityRelationsQueue { get; set; } = new();


        public ServerContinent ServiceKey { get; private set; }
        private short WorldId => ServiceKey.WorldId;
        private uint ZoneId => ServiceKey.ZoneId;

        private DateTime SeedTimestamp { get; set; }

        public override string ServiceName => $"FacilityPopGraphService_{ServiceKey}";

        private bool _initializedValue;
        private bool _disposedValue;


        public FacilityPopGraphService(IFacilityRepository facilityRepository, IFacilityStore facilityStore, ILogger<FacilityPopGraphService> logger)
        {
            _facilityRepository = facilityRepository;
            _facilityStore = facilityStore;
            _logger = logger;
        }

        public void SetServiceKey(ServerContinent serviceKey)
        {
            if (!_initializedValue)
            {
                ServiceKey = serviceKey;
                _initializedValue = true;
            }
        }

        public override async Task StartInternalAsync(CancellationToken cancellationToken)
        {
            PauseUpdates = true;

            if (!_initializedValue)
            {
                // TODO: throw new exception
                return;
            }

            await SeedFacilityGraph();

            PauseUpdates = false;

            // start processing queues
        }

        public override Task StopInternalAsync(CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }

        #region Facility Graph Seeding
        private async Task SeedFacilityGraph()
        {
            SeedTimestamp = DateTime.UtcNow;

            try
            {
                await SeedFacilityNodes();

                await SeedFacilityConnections();

                //FacilityGraph.MarkMapComplete();
                FacilityPopGraph.MarkFacilityMapComplete();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ServiceName}] Error seeding facility graph: {ex}");
            }
            finally
            {

            }
        }

        private async Task SeedFacilityNodes()
        {
            var mapRegionsTask = _facilityRepository.GetMapRegionsByZoneIdAsync(ZoneId);
            var regionOwnershipTask = GetMapOwnership(WorldId, ZoneId);

            var taskList = new List<Task>
            {
                    mapRegionsTask,
                    regionOwnershipTask
            };

            await Task.WhenAll(taskList);

            var mapRegions = mapRegionsTask.Result.ToList();
            var regionOwners = regionOwnershipTask.Result.ToList();


            foreach (var region in mapRegions)
            {
                var owner = regionOwners.Where(o => o.RegionId == region.Id).Select(o => o.FactionId).SingleOrDefault();

                var facilityNode = new FacilityNode(region, owner, SeedTimestamp);

                //FacilityGraph.AddNode(facilityNode);
                //await PlayerFacilityGraph.AddNode(facilityNode);\

                await FacilityPopGraph.AddFacility(facilityNode);
            }
        }

        private async Task SeedFacilityConnections()
        {
            var facilityLinks = await _facilityRepository.GetFacilityLinksByZoneIdAsync(ZoneId);

            foreach (var link in facilityLinks)
            {
                //FacilityGraph.AddConnection(link.FacilityIdA, link.FacilityIdB);
                FacilityPopGraph.AddFacilityConnection(link.FacilityIdA, link.FacilityIdB);
            }
        }

        private async Task<IEnumerable<ZoneRegionOwnership>> GetMapOwnership(short worldId, uint zoneId)
        {
            var mapOwnership = await _facilityStore.GetMapOwnershipAsync(worldId, zoneId);

            return mapOwnership?.Select(o => new ZoneRegionOwnership(o.Key, o.Value));
        }
        #endregion Facility Graph Seeding

        #region Stream Update Handling
        public async Task UpdateRegionOwner(FacilityOwnerUpdate update)
        {
            if (PauseUpdates)
            {
                FacilityOwnerQueue.Enqueue(update);
            }
            else
            {
                //await FacilityGraph.UpdateFacilityOwner(update);
                await FacilityPopGraph.UpdateFacilityOwner(update);
            }
        }

        public async Task UpdatePlayerRelation(PlayerRelationEvent update)
        {
            if (PauseUpdates)
            {
                PlayerRelationsQueue.Enqueue(update);
            }
            else
            {
                await FacilityPopGraph.UpdatePlayerRelation(update);
            }
        }

        public async Task UpdateFacilityRelation(FacilityRelationEvent update)
        {
            if (PauseUpdates)
            {
                FacilityRelationsQueue.Enqueue(update);
            }
            else
            {
                await FacilityPopGraph.UpdateFacilityRelation(update);
            }
        }
        #endregion Stream Update Handling

        #region Graph Counts
        public int GetFacilityCount() => FacilityPopGraph.FacilityCount;

        public int GetFacilityLinkCount() => FacilityPopGraph.FacilityLinkCount;

        public int GetPlayerCount() => FacilityPopGraph.PlayerCount;

        public int GetPlayerRelationCount() => FacilityPopGraph.PlayerRelationCount;

        public int GetFacilityRelationCount() => FacilityPopGraph.FacilityRelationCount;
        #endregion Graph Counts


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~FacilityPopGraphService()
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
