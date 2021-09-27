using Microsoft.Extensions.Hosting;
using PlanetsideSeaState.Graphing.Services;
using System.Threading;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Graphing.Services
{
    public class FacilityPopGraphServicesManagerHostedService : IHostedService
    {
        private readonly IFacilityPopGraphServicesManager _service;

        public FacilityPopGraphServicesManagerHostedService(IFacilityPopGraphServicesManager service)
        {
            _service = service;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _service.OnApplicationStartup(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _service.OnApplicationShutdown(cancellationToken);
        }
    }
}
