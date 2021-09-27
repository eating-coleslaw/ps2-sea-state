using Microsoft.Extensions.DependencyInjection;
using PlanetsideSeaState.Graphing.Models;
using PlanetsideSeaState.Graphing.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Graphing.Services
{
    public class FacilityPopGraphServiceHelper : IFacilityPopGraphServiceHelper
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public FacilityPopGraphServiceHelper(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public FacilityPopGraphServiceFactory GetFactory()
        {
            return new FacilityPopGraphServiceFactory(_scopeFactory);
        }

        public class FacilityPopGraphServiceFactory : IAsyncDisposable
        {
            private readonly IServiceScope _scope;
            private readonly FacilityPopGraphService _graphService;

            public FacilityPopGraphServiceFactory(IServiceScopeFactory scopeFactory)
            {
                _scope = scopeFactory.CreateScope();
                _graphService = _scope.ServiceProvider.GetRequiredService<FacilityPopGraphService>();
            }

            public FacilityPopGraphService GetGraphService(ServerContinent serviceKey)
            {
                _graphService.SetServiceKey(serviceKey);

                return _graphService;
            }

            public async Task<FacilityPopGraphService> GetAndStartGraphService(ServerContinent serviceKey)
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
