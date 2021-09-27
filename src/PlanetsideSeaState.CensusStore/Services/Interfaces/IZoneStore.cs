using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.CensusStore.Services
{
    public interface IZoneStore : IUpdateable
    {
        Task<IEnumerable<Zone>> GetAllZonesAsync();
        Task<Zone> GetZoneByIdAsync(int zoneId);
    }
}
