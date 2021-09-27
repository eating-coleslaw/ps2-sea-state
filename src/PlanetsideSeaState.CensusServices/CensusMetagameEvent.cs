using PlanetsideSeaState.CensusServices.Models;
using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.CensusServices
{
    public class CensusMetagameEvent
    {
        public readonly ICensusQueryFactory _queryFactory;

        public CensusMetagameEvent(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<IEnumerable<CensusMetagameEventModel>> GetAllMetagameEvents()
        {
            var query = _queryFactory.Create("metagame_event");

            return await query.GetBatchAsync<CensusMetagameEventModel>();
        }

        public async Task<CensusMetagameEventModel> GetMetagameEvent(int metagameEventId)
        {
            var query = _queryFactory.Create("metagame_event");

            query.Where("metagame_event_id").Equals(metagameEventId);

            return await query.GetAsync<CensusMetagameEventModel>();
        }

        public async Task<IEnumerable<CensusMetagameEventStateModel>> GetAllMetagameEventStates()
        {
            var query = _queryFactory.Create("metagame_event_state");

            return await query.GetBatchAsync<CensusMetagameEventStateModel>();
        }

        public async Task<CensusMetagameEventStateModel> GetMetagameEventState(int metagameEventStateId)
        {
            var query = _queryFactory.Create("metagame_event_state");

            query.Where("metagame_event_state_id").Equals(metagameEventStateId);

            return await query.GetAsync<CensusMetagameEventStateModel>();
        }
    }
}
