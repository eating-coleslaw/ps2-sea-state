using Microsoft.Extensions.Logging;
using PlanetsideSeaState.App;
using PlanetsideSeaState.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Graphing.Services.PlayersGraph
{
    public class PlayersGraphServicesManager : StatefulHostedService, IPlayersGraphServicesManager
    {
        private readonly IPlayersGraphServiceHelper _graphServiceHelper;
        private readonly ILogger<PlayersGraphServicesManager> _logger;

        public override string ServiceName => "PlayersGraphServicesManager";

        private ConcurrentDictionary<int, PlayersGraphService> GraphServices { get; set; } = new();

        private readonly KeyedSemaphoreSlim _graphServiceSemaphore = new();


        public PlayersGraphServicesManager(IPlayersGraphServiceHelper graphServiceHelper, ILogger<PlayersGraphServicesManager> logger)
        {
            _graphServiceHelper = graphServiceHelper;
            _logger = logger;
        }

        public async Task<bool> TryAddNewService(int worldId)
        {
            var key = worldId;

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

        public async Task<bool> TryRemoveService(int worldId)
        {
            var key = worldId;

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

        public PlayersGraphService GetService(int worldId)
        {
            var key = worldId;

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
            var TaskList = new List<Task>
            {
                TryAddNewService(17) // Emerald
            };

            await Task.WhenAll(TaskList);
        }

        public override async Task StopInternalAsync(CancellationToken cancellationToken)
        {
            var TaskList = new List<Task>();

            foreach (var graphServiceKey in GraphServices.Keys)
            {
                TaskList.Add(TryRemoveService(graphServiceKey));
            }

            await Task.WhenAll(TaskList);
        }
    }
}
