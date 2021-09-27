using PlanetsideSeaState.Data.Models.Census;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data.Repositories
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public CharacterRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<Character> GetCharacterByIdAsync(string characterId)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Characters.SingleOrDefaultAsync(c => c.Id == characterId);
        }

        public async Task<Character> GetCharacterByNameAsync(string characterName)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Characters.SingleOrDefaultAsync(c => c.Name == characterName);
        }

        public async Task<IEnumerable<Character>> GetCharactersById(IEnumerable<string> characterIds)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            return await dbContext.Characters.Where(c => characterIds.Contains(c.Id)).ToListAsync();
        }

        public async Task<Character> UpsertAsync(Character entity)
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            await dbContext.Characters.UpsertAsync(entity, a => a.Id == entity.Id);

            await dbContext.SaveChangesAsync();
            
            //try
            //{
            //}
            //catch (DbUpdateException ex) when ((ex.InnerException as PostgresException)?.SqlState == "23505")
            //{
            //    // Ignore unique constraint errors (https://www.postgresql.org/docs/current/static/errcodes-appendix.html)
            //}

            return entity;
        }
    }
}
