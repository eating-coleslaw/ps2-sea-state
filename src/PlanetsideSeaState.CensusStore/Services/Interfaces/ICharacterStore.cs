using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.CensusStore.Services
{
    public interface ICharacterStore
    {
        Task<Character> GetCharacterAsync(string characterId);
        Task<Character> GetCharacterByNameAsync(string characterName);
        Task<OutfitMember> GetCharacterOutfitAsync(string characterId);
        Task<IEnumerable<Character>> GetCharactersById(IEnumerable<string> characterIds);
    }
}
