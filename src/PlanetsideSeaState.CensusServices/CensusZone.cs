using PlanetsideSeaState.CensusServices.Models;
using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.CensusServices
{
    public class CensusZone
    {
        public readonly ICensusQueryFactory _queryFactory;
        
        public CensusZone(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<IEnumerable<CensusZoneModel>> GetAllZones()
        {
            var query = _queryFactory.Create("zone");
            query.SetLanguage("en");

            query.ShowFields("zone_id", "code", "name", "description", "hex_size");

            return await query.GetBatchAsync<CensusZoneModel>();
        }
    }
}
