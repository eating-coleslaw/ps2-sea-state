using PlanetsideSeaState.CensusServices;
using PlanetsideSeaState.CensusServices.Models;
using PlanetsideSeaState.Data.Models.Census;
using DaybreakGames.Census.Exceptions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanetsideSeaState.CensusStore.Services
{
    public class OutfitStore : IOutfitStore
    {
        private readonly CensusOutfit _censusOutfit;
        private readonly CensusCharacter _censusCharacter;
        private readonly ILogger<OutfitStore> _logger;

        public OutfitStore(CensusOutfit censusOutfit, CensusCharacter censusCharacter, ILogger<OutfitStore> logger)
        {
            _censusOutfit = censusOutfit;
            _censusCharacter = censusCharacter;
            _logger = logger;
        }

        public async Task<IEnumerable<Outfit>> GetOutfitsByIdsAsync(IEnumerable<string> outfitIds)
        {
            var outfitTasks = outfitIds.Select(GetOutfitByIdAsync).ToList();

            var outfits = new List<Outfit>();

            while (outfitTasks.Any())
            {
                var finishedTask = await Task.WhenAny(outfitTasks);

                var outfit = await finishedTask;

                outfitTasks.Remove(finishedTask);

                if (outfit != null)
                {
                    outfits.Add(outfit);
                }
            }

            return outfits;
        }

        public async Task<Outfit> GetOutfitByIdAsync(string outfitId)
        {
            return await GetOutfitInternalAsync(outfitId);
        }

        public async Task<Outfit> GetOutfitByAlias(string alias)
        {
            var outfit = await _censusOutfit.GetOutfitByAliasAsync(alias);

            if (outfit == null)
            {
                return null;
            }

            var censusEntity = ConvertToDbModel(outfit);

            if (censusEntity.MemberCount == 0)
            {
                return censusEntity;
            }

            var resolvedOutfit = await ResolveOutfitDetailsAsync(censusEntity, null);

            return resolvedOutfit;
        }

        public async Task<IEnumerable<Character>> GetOutfitMembersByAlias(string alias)
        {
            var members = await _censusOutfit.GetOutfitMembersByAliasAsync(alias);
            if (members == null)
            {
                return null;
            }

            var validMembers = members.Where(m => m.CharacterId != null && m.Name != null);

            var censusEntities = validMembers.Select(ConvertToDbModel);

            return censusEntities.ToList();
        }

        public async Task<OutfitMember> UpdateCharacterOutfitMembership(Character character)
        {
            OutfitMember outfitMember;
            CensusOutfitMemberModel membership;
            
            try
            {
                membership = await _censusCharacter.GetCharacterOutfitMembership(character.Id);
            }
            catch (CensusConnectionException)
            {
                return null;
            }

            if (membership == null)
            {
                return null;
            }

            var outfit = await GetOutfitInternalAsync(membership.OutfitId, character);
            if (outfit == null)
            {
                _logger.LogError(84624, $"Unable to resolve outfit {membership.OutfitId} for character {character.Id}");
                return null;
            }

            outfitMember = new OutfitMember
            {
                OutfitId = membership.OutfitId,
                CharacterId = membership.CharacterId,
                FactionId = character.FactionId,
                MemberSinceDate = membership.MemberSinceDate,
                Rank = membership.Rank,
                RankOrdinal = membership.RankOrdinal
            };

            return outfitMember;
        }
        
        private async Task<Outfit> GetOutfitInternalAsync(string outfitId, Character member = null)
        {
            Outfit outfit;
            
            try
            {
                outfit = await GetOutfitFromCensus(outfitId);
            }
            catch
            {
                return null;
            }

            if (outfit == null)
            {
                return null;
            }

            // These are null if outfit was retrieved from the census API
            if (outfit.WorldId == null || outfit.FactionId == null)
            {
                outfit = await ResolveOutfitDetailsAsync(outfit, member);
            }

            return outfit;
        }

        private async Task<Outfit> GetOutfitFromCensus(string outfitId)
        {
            var censusOutfit = await _censusOutfit.GetOutfitAsync(outfitId);

            return censusOutfit == null
                ? null
                : ConvertToDbModel(censusOutfit);
        }

        private static Outfit ConvertToDbModel(CensusOutfitModel censusOutfit)
        {
            return new Outfit
            {
                Id = censusOutfit.OutfitId,
                Alias = censusOutfit.Alias,
                AliasLower = censusOutfit.AliasLower,
                Name = censusOutfit.Name,
                LeaderCharacterId = censusOutfit.LeaderCharacterId,
                CreatedDate = censusOutfit.TimeCreated,
                MemberCount = censusOutfit.MemberCount
            };
        }

        private async Task<Outfit> ResolveOutfitDetailsAsync(Outfit outfit, Character member)
        {
            if (member != null)
            {
                outfit.WorldId = member.WorldId;
                outfit.FactionId = member.FactionId;
            }
            else
            {
                var leader = await _censusCharacter.GetCharacter(outfit.LeaderCharacterId);
                outfit.WorldId = leader.WorldId;
                outfit.FactionId = leader.FactionId;
            }

            return outfit;
        }

        private static Character ConvertToDbModel(CensusOutfitMemberCharacterModel member)
        {
            bool isOnline;

            if (int.TryParse(member.OnlineStatus, out int onlineStatus))
            {
                isOnline = onlineStatus > 0;
            }
            else // "service_unavailable"
            {
                isOnline = false;
            }

            return new Character
            {
                Id = member.CharacterId,
                Name = member.Name.First,
                IsOnline = isOnline,
                PrestigeLevel = member.PrestigeLevel,
                OutfitId = member.OutfitId,
                OutfitAlias = member.OutfitAlias,
                OutfitAliasLower = member.OutfitAliasLower
            };
        }
    }
}
