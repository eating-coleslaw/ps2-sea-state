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
    public class FacilityStore : IFacilityStore
    {
        private readonly IFacilityRepository _facilityRepository;
        private readonly CensusFacility _censusFacility;
        private readonly ILogger<FacilityStore> _logger;

        public string StoreName => "FacilityStore";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public FacilityStore(IFacilityRepository facilityRepository, CensusFacility censusFacility, ILogger<FacilityStore> logger)
        {
            _facilityRepository = facilityRepository;
            _censusFacility = censusFacility;
            _logger = logger;
        }

        // Credit to Lampjaw
        public async Task<Dictionary<int, int>> GetMapOwnershipAsync(int worldId, int zoneId)
        {
            var map = await _censusFacility.GetMapOwnership(worldId, zoneId);
            return map?.Regions.Row.ToDictionary(a => a.RowData.RegionId, a => a.RowData.FactionId);
        }

        public async Task<IEnumerable<MapRegion>> GetMapRegionByFacilityIdsAsync(params int[] facilityIds)
        {
            return await _facilityRepository.GetMapRegionsByFacilityIdsAsync(facilityIds);
        }

        public async Task<IEnumerable<MapRegion>> GetMapRegionsByFacilityTypeAsync(int facilityTypeId)
        {
            return await _facilityRepository.GetMapRegionsByFacilityTypeAsync(facilityTypeId);
        }

        public async Task<IEnumerable<MapRegion>> GetMapRegionsByZoneIdAsync(int zoneId)
        {
            return await _facilityRepository.GetMapRegionsByZoneIdAsync(zoneId);
        }

        public async Task<IEnumerable<FacilityLink>> GetFacilityLinksByZoneIdAsync(int zoneId)
        {
            return await _facilityRepository.GetFacilityLinksByZoneIdAsync(zoneId);
        }

        public async Task<IEnumerable<FacilityLink>> GetFacilityLinksByFacilityIdsAsync(params int[] facilityIds)
        {
            return await _facilityRepository.GetFacilityLinksByFacilityIdsAsync(facilityIds);
        }


        public async Task RefreshStore()
        {
            var mapRegions = await _censusFacility.GetAllMapRegions();

            var facilityLinks = await _censusFacility.GetAllFacilityLinks();

            var facilityTypes = await _censusFacility.GetAllFacilityTypes();
            
            if (mapRegions != null)
            {
                await _facilityRepository.UpsertRangeAsync(mapRegions.Select(ConvertToDbModel));
            }

            if (facilityLinks != null)
            {
                await _facilityRepository.UpsertRangeAsync(facilityLinks.Select(ConvertToDbModel));
            }

            if (facilityTypes != null)
            {
                await _facilityRepository.UpserRangeAsync(facilityTypes.Select(ConvertToDbModel));
            }
        }

        private MapRegion ConvertToDbModel(CensusMapRegionModel model)
        {
            return new MapRegion
            {
                Id = model.MapRegionId,
                FacilityId = model.FacilityId,
                FacilityName = model.FacilityName,
                FacilityTypeId = model.FacilityTypeId,
                FacilityType = model.FacilityType,
                ZoneId = model.ZoneId
            };
        }

        private FacilityLink ConvertToDbModel(CensusFacilityLinkModel model)
        {
            return new FacilityLink
            {
                ZoneId = model.ZoneId,
                FacilityIdA = model.FacilityIdA,
                FacilityIdB = model.FacilityIdB,
                Desription = model.Description
            };
        }

        private FacilityType ConvertToDbModel(CensusFacilityTypeModel censusModel)
        {
            return new FacilityType
            {
                Id = censusModel.FacilityTypeId,
                Description = censusModel.Description
            };
        }
    }
}
