using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data.Repositories
{
    public interface IFactionRepository
    {
        Task<IEnumerable<Faction>> GetAllFactionsAsync();
        Task<Faction> GetFactionByIdAsync(int factionId);
        Task UpsertRangeAsync(IEnumerable<Faction> entities);
    }
}
