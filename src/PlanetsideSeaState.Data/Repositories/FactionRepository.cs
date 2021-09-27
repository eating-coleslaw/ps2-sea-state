using Microsoft.EntityFrameworkCore;
using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data.Repositories
{
    class FactionRepository : IFactionRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public FactionRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<IEnumerable<Faction>> GetAllFactionsAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Factions.ToListAsync();
        }

        public async Task<Faction> GetFactionByIdAsync(int factionId)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Factions.Where(f => f.Id == factionId).SingleOrDefaultAsync();
        }

        public async Task UpsertRangeAsync(IEnumerable<Faction> entities)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            await dbContext.Factions.UpsertRangeAsync(entities, (a, e) => a.Id == e.Id);

            await dbContext.SaveChangesAsync();
        }
    }
}
