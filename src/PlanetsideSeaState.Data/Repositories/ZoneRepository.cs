using Microsoft.EntityFrameworkCore;
using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data.Repositories
{
    public class ZoneRepository : IZoneRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public ZoneRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }
        
        public async Task<IEnumerable<Zone>> GetAllZonesAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Zones.ToListAsync();
        }

        public async Task<Zone> GetZoneByIdAsync(int zoneId)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Zones.Where(z => z.Id == zoneId).SingleOrDefaultAsync();
        }

        public async Task UpsertRangeAsync(IEnumerable<Zone> entities)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            await dbContext.Zones.UpsertRangeAsync(entities, (a, e) => a.Id == e.Id);

            await dbContext.SaveChangesAsync();
        }
    }
}
