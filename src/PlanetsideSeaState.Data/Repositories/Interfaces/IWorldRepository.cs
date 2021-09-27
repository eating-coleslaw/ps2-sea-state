using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data.Repositories
{
    public interface IWorldRepository
    {
        Task<IEnumerable<World>> GetAllWorldsAsync();
        Task<World> GetWorldByIdAsync(int worldId);
        Task UpsertRangeAsync(IEnumerable<World> entities);
    }
}