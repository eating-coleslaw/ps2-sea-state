using Microsoft.Extensions.Logging;
using PlanetsideSeaState.App;
using PlanetsideSeaState.Graphing.Models;
using PlanetsideSeaState.Graphing.Services;
using PlanetsideSeaState.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Graphing.Services
{
    public class FacilityPopGraphServicesManager : StatefulHostedService, IFacilityPopGraphServicesManager
    {
        private readonly IFacilityPopGraphServiceHelper _graphServiceHelper;
        private readonly ILogger<FacilityPopGraphServicesManager> _logger;

        public override string ServiceName => "FacilityPopGraphServicesManager";

        private ConcurrentDictionary<ServerContinent, FacilityPopGraphService> GraphServices { get; set; } = new();

        private readonly KeyedSemaphoreSlim _graphServiceSemaphore = new();


        public FacilityPopGraphServicesManager(IFacilityPopGraphServiceHelper graphServiceHelper, ILogger<FacilityPopGraphServicesManager> logger)
        {
            _graphServiceHelper = graphServiceHelper;
            _logger = logger;
        }

        public async Task<bool> TryAddNewService(int worldId, int zoneId)
        {
            var key = new ServerContinent(worldId, zoneId);

            using (await _graphServiceSemaphore.WaitAsync(key.ToString()))
            {
                if (GraphServices.ContainsKey(key))
                {
                    return false;
                }

                var factory = _graphServiceHelper.GetFactory();

                var graphService = factory.GetGraphService(key);

                if (GraphServices.TryAdd(key, graphService))
                {
                    await graphService.StartAsync(CancellationToken.None);

                    _logger.LogInformation($"Added service {graphService.ServiceName}");

                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        public async Task<bool> TryRemoveService(int worldId, int zoneId)
        {
            var key = new ServerContinent(worldId, zoneId);

            using (await _graphServiceSemaphore.WaitAsync(key.ToString()))
            {
                if (GraphServices.TryRemove(key, out var graphService))
                {
                    await graphService.StopAsync(CancellationToken.None);

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public FacilityPopGraphService GetService(int worldId, int zoneId)
        {
            var key = new ServerContinent(worldId, zoneId);

            if (GraphServices.TryGetValue(key, out var graphService))
            {
                return graphService;
            }
            else
            {
                return null;
            }
        }

        public override async Task StartInternalAsync(CancellationToken cancellationToken)
        {
            var worldId = 17; // Emerald

            var TaskList = new List<Task>
            {
                TryAddNewService(worldId, 2), // Indar
                TryAddNewService(worldId, 4), // Amerish
                TryAddNewService(worldId, 6), // Hossin
                TryAddNewService(worldId, 8)  // Esamir
            };

            await Task.WhenAll(TaskList);
        }

        public override async Task StopInternalAsync(CancellationToken cancellationToken)
        {
            var TaskList = new List<Task>();

            foreach (var graphServiceKey in GraphServices.Keys)
            {
                TaskList.Add(TryRemoveService(graphServiceKey.WorldId, graphServiceKey.ZoneId));
            }

            await Task.WhenAll(TaskList);
        }
    }
}
