using PlanetsideSeaState.CensusStore.Services;
using PlanetsideSeaState.Data.Models.Census;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.Services.Planetside
{
    public class OutfitService : IOutfitService
    {
        private readonly IOutfitStore _outfitStore;
        private readonly ICharacterStore _characterStore;
        private readonly ILogger<OutfitService> _logger;

        public OutfitService(IOutfitStore outfitStore, ICharacterStore characterStore, ILogger<OutfitService> logger)
        {
            _outfitStore = outfitStore;
            _characterStore = characterStore;
            _logger = logger;
        }

        public async Task<IEnumerable<Outfit>> GetOutfitsByIdsAsync(IEnumerable<string> outfitIds)
        {
            return await _outfitStore.GetOutfitsByIdsAsync(outfitIds);
        }

        public async Task<Outfit> GetOutfit(string outfitId)
        {
            return await _outfitStore.GetOutfitByIdAsync(outfitId);
        }

        public async Task<Outfit> GetOutfitByAlias(string alias)
        {
            return await _outfitStore.GetOutfitByAlias(alias);
        }

        public async Task<IEnumerable<Character>> GetOutfitMembersByAlias(string alias)
        {
            return await _outfitStore.GetOutfitMembersByAlias(alias);
        }

        public async Task<OutfitMember> GetUpdatedCharacterOutfitMembership(Character character)
        {
            return await _outfitStore.UpdateCharacterOutfitMembership(character);
        }
    }
}
