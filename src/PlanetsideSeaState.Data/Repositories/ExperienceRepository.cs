using PlanetsideSeaState.Data.Models.Census;
using Microsoft.EntityFrameworkCore;
using PlanetsideSeaState.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data.Repositories
{
    public class ExperienceRepository : IExperienceRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public ExperienceRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<IEnumerable<Experience>> GetAllExperiencesAsync()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Experiences.ToListAsync();
        }

        public async Task<Experience> GetExperienceAsync(int experienceId)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Experiences.Where(e => e.Id == experienceId).FirstOrDefaultAsync();
        }

        public async Task UpsertRangeAsync(IEnumerable<Experience> entities)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            await dbContext.Experiences.UpsertRangeAsync(entities, (a, e) => a.Id == e.Id);

            await dbContext.SaveChangesAsync();
        }
    }
}
