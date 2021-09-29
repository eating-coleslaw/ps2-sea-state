using PlanetsideSeaState.App;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Graphing.Services.FacilityPopsGraph
{
    public interface IFacilityPopGraphServicesManager : IStatefulHostedService
    {
        FacilityPopGraphService GetService(int worldId, int zoneId);

        Task<bool> TryAddNewService(int worldId, int zoneId);
        Task<bool> TryRemoveService(int worldId, int zoneId);
    }
}
