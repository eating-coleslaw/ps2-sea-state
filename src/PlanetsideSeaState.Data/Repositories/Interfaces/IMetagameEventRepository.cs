using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data.Repositories
{
    public interface IMetagameEventRepository
    {
        Task<IEnumerable<MetagameEventCategory>> GetAllMetagameEventsAsync();
        Task<MetagameEventCategory> GetMetagameEventAsync(int eventId);
        Task<MetagameEventState> GetMetagameEventStateAsync(int stateId);
        Task UpsertRangeAsync(IEnumerable<MetagameEventCategory> entities);
        Task UpsertRangeAsync(IEnumerable<MetagameEventState> entities);
    }
}
