using PlanetsideSeaState.Data.Models.Events;
using PlanetsideSeaState.Data.Models.QueryResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data.Repositories
{
    public interface IEventRepository
    {
        Task AddAsync<T>(T entity) where T : class;
        Task<IEnumerable<Death>> GetDeathsForWorldInTimeRange(short worldId, DateTime start, DateTime end);
        Task<IEnumerable<DeathWithExperience>> GetDeathWithExperienceForWorldInTimeRange(short worldId, DateTime start, DateTime end);
        Task<IEnumerable<FacilityControlInfo>> GetRecentFacilityControlsAsync(short? worldId, int? facilityId, short? rowLimit);
    }
}
