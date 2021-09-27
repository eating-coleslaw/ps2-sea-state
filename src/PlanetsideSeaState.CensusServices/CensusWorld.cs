using PlanetsideSeaState.CensusServices.Models;
using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.CensusServices
{
    public class CensusWorld
    {
        private readonly ICensusQueryFactory _queryFactory;

        public CensusWorld(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<IEnumerable<CensusWorldModel>> GetAllWorlds()
        {
            var query = _queryFactory.Create("world");
            query.SetLanguage("en");

            return await query.GetBatchAsync<CensusWorldModel>();
        }
    }
}
