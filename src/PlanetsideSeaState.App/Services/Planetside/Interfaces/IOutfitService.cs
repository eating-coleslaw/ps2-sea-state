using PlanetsideSeaState.Data.Models.Census;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.Services.Planetside
{
    public interface IOutfitService
    {
        Task<Outfit> GetOutfit(string outfitId);
        Task<Outfit> GetOutfitByAlias(string alias);
        Task<IEnumerable<Character>> GetOutfitMembersByAlias(string alias);
        Task<IEnumerable<Outfit>> GetOutfitsByIdsAsync(IEnumerable<string> outfitIds);
        Task<OutfitMember> GetUpdatedCharacterOutfitMembership(Character character);

    }
}
