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
    public class ZoneStore : IZoneStore
    {
        private readonly IZoneRepository _zoneRepository;
        private readonly CensusZone _censusZone;
        private readonly ILogger<ZoneStore> _logger;

        public string StoreName => "ZoneStore";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public ZoneStore(IZoneRepository zoneRepository, CensusZone censusZone, ILogger<ZoneStore> logger)
        {
            _zoneRepository = zoneRepository;
            _censusZone = censusZone;
            _logger = logger;
        }

        public async Task<IEnumerable<Zone>> GetAllZonesAsync()
        {
            return await _zoneRepository.GetAllZonesAsync();
        }

        public async Task<Zone> GetZoneByIdAsync(int zoneId)
        {
            return await _zoneRepository.GetZoneByIdAsync(zoneId);
        }

        public async Task RefreshStore()
        {
            var zones = await _censusZone.GetAllZones();

            if (zones != null)
            {
                await _zoneRepository.UpsertRangeAsync(zones.Select(ConvertToDbModel));
            }
        }

        private static Zone ConvertToDbModel(CensusZoneModel censusModel)
        {
            return new Zone
            {
                Id = censusModel.ZoneId,
                Name = censusModel.Name.English,
                Description = censusModel.Description.English,
                Code = censusModel.Code,
                HexSize = censusModel.HexSize
            };
        }
    }
}
