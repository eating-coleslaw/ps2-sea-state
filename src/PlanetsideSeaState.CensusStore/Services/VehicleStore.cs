using PlanetsideSeaState.CensusServices;
using PlanetsideSeaState.CensusServices.Models;
using PlanetsideSeaState.Data.Models.Census;
using PlanetsideSeaState.Data.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanetsideSeaState.CensusStore.Services
{
    public class VehicleStore : IVehicleStore
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly CensusVehicle _censusVehicle;
        private readonly ILogger<VehicleStore> _logger;

        public string StoreName => "VehicleStore";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public VehicleStore(IVehicleRepository vehicleRepository, CensusVehicle censusVehicle, ILogger<VehicleStore> logger)
        {
            _vehicleRepository = vehicleRepository;
            _censusVehicle = censusVehicle;
            _logger = logger;
        }

        public async Task<Vehicle> GetVehicleByIdAsync(int vehicleId)
        {
            return await _vehicleRepository.GetVehicleByIdAsync(vehicleId);
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
        {
            return await _vehicleRepository.GetAllVehiclesAsync();
        }


        public async Task RefreshStore()
        {
            var vehicles = await _censusVehicle.GetAllVehicles();

            if (vehicles != null)
            {
                await _vehicleRepository.UpsertRangeAsync(vehicles.Select(ConvertToDbModel));
            }
        }

        private static Vehicle ConvertToDbModel(CensusVehicleModel censusModel)
        {
            return new Vehicle
            {
                Id = censusModel.VehicleId,
                Name = censusModel.Name?.English,
                Description = censusModel.Description?.English,
                TypeId = censusModel.TypeId,
                TypeName = censusModel.TypeName,
                Cost = censusModel.Cost,
                CostResourceId = censusModel.CostResourceId,
                ImageId = censusModel.ImageId
            };
        }
    }
}
