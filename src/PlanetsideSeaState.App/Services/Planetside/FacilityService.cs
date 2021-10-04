using PlanetsideSeaState.CensusStore.Services;
using PlanetsideSeaState.Data.Models.Census;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlanetsideSeaState.Shared.Planetside;

namespace PlanetsideSeaState.App.Services.Planetside
{
    public class FacilityService : IFacilityService
    {
        private readonly IFacilityStore _facilityStore;
        private readonly ILogger<FacilityService> _logger;

        public FacilityService(IFacilityStore facilityStore, ILogger<FacilityService> logger)
        {
            _facilityStore = facilityStore;
            _logger = logger;
        }

        public async Task<IEnumerable<ZoneRegionOwnership>> GetMapOwnership(short worldId, uint zoneId)
        {
            var mapOwnership = await _facilityStore.GetMapOwnershipAsync(worldId, zoneId);

            return mapOwnership?.Select(o => new ZoneRegionOwnership(o.Key, o.Value));
        }

        public async Task<IEnumerable<MapRegion>> GetMapRegionsByZoneIdAsync(uint zoneId)
        {
            return await _facilityStore.GetMapRegionsByZoneIdAsync(zoneId);
        }

        public async Task<IEnumerable<FacilityLink>> GetFacilityLinksByZoneIdAsync(uint zoneId)
        {
            return await _facilityStore.GetFacilityLinksByZoneIdAsync(zoneId);
        }

        public Task<MapRegion> GetMapRegion(int mapRegionId)
        {
            throw new System.NotImplementedException();
        }

        public Task<MapRegion> GetMapRegionFromFacilityId(int facilityId)
        {
            throw new System.NotImplementedException();
        }

        public Task<MapRegion> GetMapRegionFromFacilityName(string facilityName)
        {
            throw new System.NotImplementedException();
        }

        public Task<MapRegion> GetMapRegionsByFacilityType(int facilityTypeId)
        {
            throw new System.NotImplementedException();
        }
    }
}
