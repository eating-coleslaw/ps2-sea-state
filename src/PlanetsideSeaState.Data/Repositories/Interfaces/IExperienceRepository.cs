using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data.Repositories
{
    public interface IExperienceRepository
    {
        Task<IEnumerable<Experience>> GetAllExperiencesAsync();
        Task<Experience> GetExperienceAsync(int experienceId);
        Task UpsertRangeAsync(IEnumerable<Experience> entities);
    }
}
