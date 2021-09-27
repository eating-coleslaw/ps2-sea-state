using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data.Repositories
{
    public interface IZoneRepository
    {
        Task<IEnumerable<Zone>> GetAllZonesAsync();
        Task<Zone> GetZoneByIdAsync(int zoneId);
        Task UpsertRangeAsync(IEnumerable<Zone> entities);
    }
}
