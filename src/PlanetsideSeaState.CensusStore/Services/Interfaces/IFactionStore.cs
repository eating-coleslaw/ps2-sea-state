using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.CensusStore.Services
{
    public interface IFactionStore : IUpdateable
    {
        Task<IEnumerable<Faction>> GetAllFactionsAsync();
        Task<Faction> GetFactionByIdAsync(int factionId);
    }
}