﻿using PlanetsideSeaState.Data.Models.Census;
using PlanetsideSeaState.App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.Services.Planetside
{
    public interface IFacilityService
    {
        Task<IEnumerable<FacilityLink>> GetFacilityLinksByZoneIdAsync(uint zoneId);
        Task<IEnumerable<ZoneRegionOwnership>> GetMapOwnership(short worldId, uint zoneId);
        Task<MapRegion> GetMapRegion(int mapRegionId);
        Task<MapRegion> GetMapRegionFromFacilityId(int facilityId);
        Task<MapRegion> GetMapRegionFromFacilityName(string facilityName);

        Task<MapRegion> GetMapRegionsByFacilityType(int facilityTypeId);
        Task<IEnumerable<MapRegion>> GetMapRegionsByZoneIdAsync(uint zoneId);
    }
}
