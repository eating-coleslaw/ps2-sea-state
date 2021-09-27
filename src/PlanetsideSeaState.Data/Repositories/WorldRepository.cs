using Microsoft.EntityFrameworkCore;
using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data.Repositories
{
    public class WorldRepository : IWorldRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public WorldRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<IEnumerable<World>> GetAllWorldsAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Worlds.ToListAsync();
        }

        public async Task<World> GetWorldByIdAsync(int worldId)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Worlds.Where(w => w.Id == worldId).SingleOrDefaultAsync();
        }

        public async Task UpsertRangeAsync(IEnumerable<World> entities)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            await dbContext.Worlds.UpsertRangeAsync(entities, (a, e) => a.Id == e.Id);

            await dbContext.SaveChangesAsync();
        }
    }
}
