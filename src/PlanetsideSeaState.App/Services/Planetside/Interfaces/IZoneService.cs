using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.Services.Planetside
{
    public interface IZoneService
    {
        Task<IEnumerable<Zone>> GetAllZones();
        Task<Zone> GetZone(int zoneId);
    }
}
