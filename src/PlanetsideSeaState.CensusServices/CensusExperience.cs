using PlanetsideSeaState.CensusServices.Models;
using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.CensusServices
{
    public class CensusExperience
    {
        public readonly ICensusQueryFactory _queryFactory;

        public CensusExperience(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<IEnumerable<CensusExperienceModel>> GetAllExperiences()
        {
            var query = _queryFactory.Create("experience");

            return await query.GetBatchAsync<CensusExperienceModel>();
        }

        public async Task<CensusExperienceModel> GetExperience(int experienceId)
        {
            var query = _queryFactory.Create("experience");

            query.Where("experience_id").Equals(experienceId);

            return await query.GetAsync<CensusExperienceModel>();
        }

    }
}
