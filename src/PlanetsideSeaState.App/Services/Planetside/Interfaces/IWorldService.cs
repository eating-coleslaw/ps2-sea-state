using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.Services.Planetside
{
    public interface IWorldService
    {
        Task<IEnumerable<World>> GetAllWorlds();
        Task<World> GetWorld(int worldId);
    }
}
