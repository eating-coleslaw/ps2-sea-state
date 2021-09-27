using PlanetsideSeaState.CensusServices;
using PlanetsideSeaState.CensusServices.Models;
using PlanetsideSeaState.Data.Models.Census;
using PlanetsideSeaState.Data.Repositories;
using PlanetsideSeaState.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.CensusStore.Services
{
    public class CharacterStore : ICharacterStore
    {
        private readonly IOutfitStore _outfitStore;
        private readonly ICharacterRepository _characterRepository;
        private readonly CensusCharacter _censusCharacter;
        private readonly ILogger<CharacterStore> _logger;

        private readonly KeyedSemaphoreSlim _characterLock = new();


        public CharacterStore(IOutfitStore outfitStore, ICharacterRepository characterRepository, CensusCharacter censusCharacter, ILogger<CharacterStore> logger)
        {
            _outfitStore = outfitStore;
            _characterRepository = characterRepository;
            _censusCharacter = censusCharacter;
            _logger = logger;
        }

        public async Task<Character> GetCharacterAsync(string characterId)
        {
            using (await _characterLock.WaitAsync(characterId))
            {
                try
                {
                    var censusEntity = await _characterRepository.GetCharacterByIdAsync(characterId);

                    if (censusEntity != null)
                    {
                        return censusEntity;
                    }

                    var character = await _censusCharacter.GetCharacter(characterId);

                    if (character == null)
                    {
                        return null;
                    }

                    censusEntity = ConvertToDbModel(character);

                    await _characterRepository.UpsertAsync(censusEntity);

                    return censusEntity;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"failed to get character {characterId}: {ex}");

                    return null;
                }
            }
        }

        public async Task<Character> GetCharacterByNameAsync(string characterName)
        {
            var censusEntity = await _characterRepository.GetCharacterByNameAsync(characterName);

            if (censusEntity != null)
            {
                return censusEntity;
            }
            
            var character = await _censusCharacter.GetCharacterByName(characterName);

            if (character == null)
            {
                return null;
            }

            censusEntity = ConvertToDbModel(character);

            await _characterRepository.UpsertAsync(censusEntity);

            return censusEntity;
        }

        public async Task<OutfitMember> GetCharacterOutfitAsync(string characterId)
        {
            var character = await GetCharacterAsync(characterId);
            if (character == null)
            {
                return null;
            }

            return await _outfitStore.UpdateCharacterOutfitMembership(character);
        }

        public async Task<IEnumerable<Character>> GetCharactersById(IEnumerable<string> characterIds)
        {
            return await _characterRepository.GetCharactersById(characterIds);
        }

        private static Character ConvertToDbModel(CensusCharacterModel censusModel)
        {
            bool isOnline;
            
            if (int.TryParse(censusModel.OnlineStatus, out int onlineStatus))
            {
                isOnline = onlineStatus > 0;
            }
            else // "service_unavailable"
            {
                isOnline = false;
            }

            return new Character
            {
                Id = censusModel.CharacterId,
                Name = censusModel.Name.First,
                FactionId = censusModel.FactionId,
                TitleId = censusModel.TitleId,
                WorldId = censusModel.WorldId,
                BattleRank = censusModel.BattleRank.Value,
                BattleRankPercentToNext = censusModel.BattleRank.PercentToNext,
                CertsEarned = censusModel.Certs.EarnedPoints,
                PrestigeLevel = censusModel.PrestigeLevel,
                IsOnline = isOnline,
                OutfitId = censusModel.OutfitMember?.OutfitId
            };
        }
    }
}
