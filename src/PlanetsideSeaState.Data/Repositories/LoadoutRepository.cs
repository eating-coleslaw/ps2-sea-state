using Microsoft.EntityFrameworkCore;
using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data.Repositories
{
    public class LoadoutRepository : ILoadoutRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public LoadoutRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<IEnumerable<Loadout>> GetAllLoadoutsAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Loadouts.ToListAsync();
        }

        public async Task UpsertRangeAsync(IEnumerable<Loadout> entities)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            await dbContext.Loadouts.UpsertRangeAsync(entities, (a, e) => a.Id == e.Id);

            await dbContext.SaveChangesAsync();
        }
    }
}
