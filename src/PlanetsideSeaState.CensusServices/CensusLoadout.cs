using PlanetsideSeaState.CensusServices.Models;
using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.CensusServices
{
    public class CensusLoadout
    {
        private readonly ICensusQueryFactory _queryFactory;

        public CensusLoadout(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<IEnumerable<CensusLoadoutModel>> GetAllLoadoutsAsync()
        {
            var query = _queryFactory.Create("loadout");

            query.ShowFields("loadout_id", "profile_id", "faction_id", "code_name");

            return await query.GetBatchAsync<CensusLoadoutModel>();
        }
    }
}
