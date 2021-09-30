using PlanetsideSeaState.App;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Graphing.Services.FacilityPopsGraph
{
    public interface IFacilityPopGraphServicesManager : IStatefulHostedService
    {
        FacilityPopGraphService GetService(short worldId, uint zoneId);

        Task<bool> TryAddNewService(short worldId, uint zoneId);
        Task<bool> TryRemoveService(short worldId, uint zoneId);
    }
}
