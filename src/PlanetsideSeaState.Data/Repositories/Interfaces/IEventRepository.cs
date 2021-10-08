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
        Task<IEnumerable<PlayerFacilityControlProximity>> GetClosestPlayerFacilityControlsAsync(DateTime start, DateTime end, short worldId, uint zoneId);
        Task<IEnumerable<Death>> GetDeathsForWorldInTimeRange(short worldId, DateTime start, DateTime end);
        Task<IEnumerable<DeathWithExperience>> GetDeathWithExperienceForWorldInTimeRange(short worldId, DateTime start, DateTime end);
        Task<FacilityControl> GetFacilityControl(int facilityId, DateTime timestamp, short worldId);
        Task<FacilityControl> GetFacilityControlAsync(Guid id);
        Task<IEnumerable<PlayerFacilityControl>> GetFacilityControlAttributedPlayers(Guid id);
        Task<IEnumerable<PlayerFacilityControl>> GetFacilityControlAttributedPlayers(int facilityId, DateTime timestamp, short worldId);
        Task<FacilityControl> GetFacilityControlWithAttributedPlayers(int facilityId, DateTime timestamp, short worldId);
        Task<FacilityControl> GetFacilityControlWithAttributedPlayers(Guid id);
        Task<IEnumerable<PlayerConnectionEvent>> GetPlayerConnectionEventsAsync(DateTime start, DateTime end, short worldId, uint? zoneId);
        Task<IEnumerable<FacilityControlInfo>> GetRecentFacilityControlsAsync(short? worldId, int? facilityId, short? rowLimit);
    }
}
