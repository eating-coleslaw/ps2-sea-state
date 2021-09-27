using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data.Repositories
{
    public interface IProfileRepository
    {
        Task<IEnumerable<Profile>> GetAllProfilesAsync();
        Task UpsertRangeAsync(IEnumerable<Profile> entities);
    }
}
