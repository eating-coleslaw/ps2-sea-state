using PlanetsideSeaState.CensusStore.Services;
using PlanetsideSeaState.Data.Models.Census;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.Services.Planetside
{
    public class ZoneService : IZoneService
    {
        private readonly IZoneStore _zoneStore;
        private readonly ILogger<ZoneService> _logger;


        public ZoneService(IZoneStore zoneStore, ILogger<ZoneService> logger)
        {
            _zoneStore = zoneStore;
            _logger = logger;
        }

        public async Task<IEnumerable<Zone>> GetAllZones()
        {
            return await _zoneStore.GetAllZonesAsync();
        }

        public async Task<Zone> GetZone(int zoneId)
        {
            return await _zoneStore.GetZoneByIdAsync(zoneId);
        }
    }
}
