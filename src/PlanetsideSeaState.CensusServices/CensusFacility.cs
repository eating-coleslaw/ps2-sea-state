using DaybreakGames.Census;
using PlanetsideSeaState.CensusServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanetsideSeaState.CensusServices
{
    public class CensusFacility
    {
        public readonly ICensusQueryFactory _queryFactory;

        public CensusFacility(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        // Credit to Lampjaw
        public async Task<CensusMapModel> GetMapOwnership(int worldId, int zoneId)
        {
            var query = _queryFactory.Create("map");
            query.SetLanguage("en");

            query.Where("world_id").Equals(worldId);
            query.Where("zone_ids").Equals(zoneId);

            try
            {
                return await query.GetAsync<CensusMapModel>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<CensusMapRegionModel>> GetAllMapRegions()
        {
            var query = _queryFactory.Create("map_region");

            query.ShowFields("map_region_id", "zone_id", "facility_id", "facility_name", "facility_type_id", "facility_type");

            return await query.GetBatchAsync<CensusMapRegionModel>();
        }

        public async Task<IEnumerable<CensusFacilityTypeModel>> GetAllFacilityTypes()
        {
            var query = _queryFactory.Create("facility_type");

            query.ShowFields("facility_type_id", "description");

            query.SetLimit(100);

            return await query.GetBatchAsync<CensusFacilityTypeModel>();
        }

        public async Task<IEnumerable<CensusFacilityLinkModel>> GetAllFacilityLinks()
        {
            var query = _queryFactory.Create("facility_link");

            return await query.GetBatchAsync<CensusFacilityLinkModel>();
        }

        public async Task<IEnumerable<CensusFacilityLinkModel>> GetZoneFacilityLinks(int zoneId)
        {
            var query = _queryFactory.Create("facility_link");

            query.Where("zone_id").Equals(zoneId);

            return await query.GetBatchAsync<CensusFacilityLinkModel>();
        }

        public async Task<IEnumerable<CensusFacilityLinkModel>> GetFacilityLinks(int facilityId)
        {
            var aLinksTask = GetFacilityALinks(facilityId);
            var bLinksTask = GetFacilityBLinks(facilityId);

            await Task.WhenAll(aLinksTask, bLinksTask);

            var aLinks = aLinksTask.Result.ToList();
            var bLinks = bLinksTask.Result.ToList();

            var facilityLinks = new List<CensusFacilityLinkModel>();

            if (aLinks != null)
            {
                facilityLinks.AddRange(aLinks);
            }

            if (bLinks != null)
            {
                facilityLinks.AddRange(bLinks);
            }

            return facilityLinks;
        }

        private async Task<IEnumerable<CensusFacilityLinkModel>> GetFacilityALinks(int facilityId)
        {
            var query = _queryFactory.Create("facility_link");

            query.Where("facility_id_a").Equals(facilityId);

            return await query.GetBatchAsync<CensusFacilityLinkModel>();
        }

        private async Task<IEnumerable<CensusFacilityLinkModel>> GetFacilityBLinks(int facilityId)
        {
            var query = _queryFactory.Create("facility_link");

            query.Where("facility_id_b").Equals(facilityId);

            return await query.GetBatchAsync<CensusFacilityLinkModel>();
        }
    }
}
