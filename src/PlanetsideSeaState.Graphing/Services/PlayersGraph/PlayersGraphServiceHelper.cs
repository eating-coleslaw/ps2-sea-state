using Microsoft.Extensions.DependencyInjection;
using PlanetsideSeaState.Graphing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Graphing.Services.PlayersGraph
{
    public class PlayersGraphServiceHelper : IPlayersGraphServiceHelper
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public PlayersGraphServiceHelper(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public PlayersGraphServiceFactory GetFactory()
        {
            return new PlayersGraphServiceFactory(_scopeFactory);
        }

        public class PlayersGraphServiceFactory : IAsyncDisposable
        {
            private readonly IServiceScope _scope;
            private readonly PlayersGraphService _graphService;

            public PlayersGraphServiceFactory(IServiceScopeFactory scopeFactory)
            {
                _scope = scopeFactory.CreateScope();
                _graphService = _scope.ServiceProvider.GetRequiredService<PlayersGraphService>();
            }

            public PlayersGraphService GetGraphService(int serviceKey)
            {
                _graphService.SetServiceKey(serviceKey);

                return _graphService;
            }

            public async Task<PlayersGraphService> GetAndStartGraphService(int serviceKey)
            {
                _graphService.SetServiceKey(serviceKey);
                await _graphService.StartAsync(CancellationToken.None);
                return _graphService;
            }

            public async ValueTask DisposeAsync()
            {
                await _graphService.StopAsync(CancellationToken.None);
                _graphService.Dispose();
                _scope.Dispose();
            }
        }
    }
}
