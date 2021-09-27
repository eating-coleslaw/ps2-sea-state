using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data.Repositories
{
    public interface ICharacterRepository
    {
        Task<Character> GetCharacterByIdAsync(string characterId);
        Task<Character> GetCharacterByNameAsync(string characterName);
        Task<IEnumerable<Character>> GetCharactersById(IEnumerable<string> characterIds);
        Task<Character> UpsertAsync(Character entity);
    }
}
