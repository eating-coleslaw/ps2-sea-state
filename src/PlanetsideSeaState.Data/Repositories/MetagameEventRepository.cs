using PlanetsideSeaState.Data.Models.Census;
using Microsoft.EntityFrameworkCore;
using PlanetsideSeaState.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data.Repositories
{
    public class MetagameEventRepository : IMetagameEventRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public MetagameEventRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<IEnumerable<MetagameEventCategory>> GetAllMetagameEventsAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.MetagameEventCategories.ToListAsync();
        }

        public async Task<MetagameEventCategory> GetMetagameEventAsync(int eventId)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.MetagameEventCategories.Where(e => e.Id == eventId).FirstOrDefaultAsync();
        }

        public async Task UpsertRangeAsync(IEnumerable<MetagameEventCategory> entities)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            await dbContext.MetagameEventCategories.UpsertRangeAsync(entities, (a, e) => a.Id == e.Id);

            await dbContext.SaveChangesAsync();
        }

        public async Task<MetagameEventState> GetMetagameEventStateAsync(int stateId)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.MetagameEventStates.Where(e => e.Id == stateId).FirstOrDefaultAsync();
        }

        public async Task UpsertRangeAsync(IEnumerable<MetagameEventState> entities)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            await dbContext.MetagameEventStates.UpsertRangeAsync(entities, (a, e) => a.Id == e.Id);

            await dbContext.SaveChangesAsync();
        }
    }
}
