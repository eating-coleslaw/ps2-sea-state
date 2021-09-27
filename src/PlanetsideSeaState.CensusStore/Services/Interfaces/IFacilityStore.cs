using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.CensusStore.Services
{
    public interface IFacilityStore : IUpdateable
    {
        Task<IEnumerable<MapRegion>> GetMapRegionByFacilityIdsAsync(params int[] facilityIds);
        Task<IEnumerable<MapRegion>> GetMapRegionsByFacilityTypeAsync(int facilityTypeId);
        Task<IEnumerable<MapRegion>> GetMapRegionsByZoneIdAsync(int zoneId);

        Task<IEnumerable<FacilityLink>> GetFacilityLinksByZoneIdAsync(int zoneId);
        Task<IEnumerable<FacilityLink>> GetFacilityLinksByFacilityIdsAsync(params int[] facilityIds);
        Task<Dictionary<int, int>> GetMapOwnershipAsync(int worldId, int zoneId);
    }
}
