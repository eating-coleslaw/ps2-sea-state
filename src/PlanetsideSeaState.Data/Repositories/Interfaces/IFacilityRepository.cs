using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data.Repositories
{
    public interface IFacilityRepository
    {
        Task<IEnumerable<MapRegion>> GetAllMapRegionsAsync();
        Task<IEnumerable<MapRegion>> GetMapRegionsByZoneIdAsync(uint zoneId);
        Task<IEnumerable<MapRegion>> GetMapRegionsByFacilityTypeAsync(int facilityTypeId);
        Task<IEnumerable<MapRegion>> GetMapRegionsByFacilityIdsAsync(IEnumerable<int> facilityIds);
        
        Task<IEnumerable<FacilityLink>> GetFacilityLinksByZoneIdAsync(uint zoneId);
        Task<IEnumerable<FacilityLink>> GetFacilityLinksByFacilityIdsAsync(IEnumerable<int> facilityIds);
        
        Task UpsertRangeAsync(IEnumerable<MapRegion> censusEntities);
        Task UpsertRangeAsync(IEnumerable<FacilityLink> entities);
        Task UpserRangeAsync(IEnumerable<FacilityType> entities);
    }
}
