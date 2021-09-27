using PlanetsideSeaState.Data.Models.Census;
using PlanetsideSeaState.Shared.Planetside;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.Services.Planetside
{
    public interface IProfileService
    {
        Task<IEnumerable<Profile>> GetAllProfiles();
        string GetPlanetsideClassDisplayName(PlanetsideClass planetsideClass);
        PlanetsideClass GetPlanetsideClassFromLoadoutId(int loadoutId);
        Task<Profile> GetProfileFromLoadoutId(int loadoutId);
    }
}
