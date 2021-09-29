using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Graphing.Services.FacilityPopsGraph
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
