using PlanetsideSeaState.CensusStore.Services;
using PlanetsideSeaState.Data.Models.Census;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.Services.Planetside
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleStore _vehicleStore;
        private readonly ILogger<VehicleService> _logger;


        public VehicleService(IVehicleStore vehicleStore, ILogger<VehicleService> logger)
        {
            _vehicleStore = vehicleStore;
            _logger = logger;
        }

        public async Task<Vehicle> GetVehicleInfo(int vehicleId)
        {
            return await _vehicleStore.GetVehicleByIdAsync(vehicleId);
        }
    }
}
