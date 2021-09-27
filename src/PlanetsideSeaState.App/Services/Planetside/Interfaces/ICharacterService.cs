using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.Services.Planetside
{
    public interface ICharacterService
    {
        Task<Character> GetCharacter(string characterId);
        Task<Character> GetCharacterByName(string characterName);
        Task<OutfitMember> GetCharacterOutfit(string characterId);
        Task<IEnumerable<Character>> GetCharactersById(IEnumerable<string> characterIds);
    }
}
