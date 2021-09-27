using PlanetsideSeaState.Data.Models.Census;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.Services.Planetside
{
    public interface IVehicleService
    {
        Task<Vehicle> GetVehicleInfo(int vehicleId);
    }
}
