// Credit to Lampjaw

using PlanetsideSeaState.Data.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data.Repositories
{
    public class UpdaterSchedulerRepository : IUpdaterSchedulerRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public UpdaterSchedulerRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public UpdaterScheduler GetUpdaterHistoryByServiceName(string serviceName)
        {
            using var factory = _dbContextHelper.GetFactory();

            var dbContext = factory.GetDbContext();

            return dbContext.UpdaterScheduler.SingleOrDefault(u => u.Id == serviceName);
        }

        public async Task UpsertAsync(UpdaterScheduler entity)
        {
            using var factory = _dbContextHelper.GetFactory();

            var dbContext = factory.GetDbContext();

            await dbContext.UpdaterScheduler.UpsertAsync(entity, a => a.Id == entity.Id);

            await dbContext.SaveChangesAsync();
        }
    }
}
