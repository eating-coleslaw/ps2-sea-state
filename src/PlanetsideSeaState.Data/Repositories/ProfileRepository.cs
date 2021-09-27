using Microsoft.EntityFrameworkCore;
using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public ProfileRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<IEnumerable<Profile>> GetAllProfilesAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Profiles.ToListAsync();
        }

        public async Task UpsertRangeAsync(IEnumerable<Profile> entities)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            await dbContext.Profiles.UpsertRangeAsync(entities, (a, e) => a.Id == e.Id);

            await dbContext.SaveChangesAsync();
        }
    }
}