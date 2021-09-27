using PlanetsideSeaState.App.Models;
using PlanetsideSeaState.Data.Models.Census;
using PlanetsideSeaState.Data.Models.Events;
using PlanetsideSeaState.Data.Models.QueryResults;
using PlanetsideSeaState.Data.Repositories;
using PlanetsideSeaState.Shared.Planetside;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.Services.Planetside
{
    public class EventService : IEventService
    {
        public readonly IEventRepository _eventRepository;
        public readonly ICharacterService _characterService;
        public readonly IOutfitService _outfitService;
        public readonly ILogger<EventService> _logger;

        public EventService(IEventRepository eventRepository, ICharacterService characterService, IOutfitService outfitService, ILogger<EventService> logger)
        {
            _eventRepository = eventRepository;
            _characterService = characterService;
            _outfitService = outfitService;
            _logger = logger;
        }

        public async Task<ActivityLeaderboardStats> GetWorldPlayerStatsInTimeRange(int worldId, DateTime start, DateTime end, int take = 20)
        {
            //var worldDeaths = await _eventRepository.GetDeathsForWorldInTimeRange(worldId, start, end);
            var worldDeaths = await _eventRepository.GetDeathWithExperienceForWorldInTimeRange(worldId, start, end);

            if (worldDeaths == null)
            {
                return null;
            }

            var playersMap = new Dictionary<string, ActivityLeaderboardPlayerStats>();

            foreach (var death in worldDeaths)
            {
                var attackerId = death.AttackerCharacterId;
                var victimId = death.VictimCharacterId;

                if (!playersMap.TryGetValue(attackerId, out var attackerStats))
                {
                    attackerStats = new ActivityLeaderboardPlayerStats();

                    playersMap.Add(attackerId, attackerStats);

                    attackerStats.Id = attackerId;
                }

                if (!playersMap.TryGetValue(victimId, out var victimStats))
                {
                    victimStats = new ActivityLeaderboardPlayerStats();

                    playersMap.Add(victimId, victimStats);

                    victimStats.Id = victimId;
                }

                if (death.AttackerFactionId != null)
                {
                    attackerStats.FactionId = death.AttackerFactionId; 
                }

                if (death.VictimFactionId != null)
                {
                    victimStats.FactionId = death.VictimFactionId;
                }

                var deathType = GetDeathType(death);

                if (deathType == DeathType.Suicide)
                {
                    victimStats.Deaths++;
                    victimStats.Suicides++;
                }
                else if (deathType == DeathType.Teamkill)
                {
                    attackerStats.Teamkills++;

                    victimStats.Deaths++;

                    if (attackerStats.FactionId != null && attackerStats.FactionId == 4)
                    {
                        victimStats.NsoTeamkillDeaths++;
                    }
                    else
                    {
                        victimStats.NonNsoTeamkillDeaths++;
                    }

                    if (victimStats.FactionId != null && victimStats.FactionId == 4)
                    {
                        attackerStats.NsoTeamkills++;
                    }
                    else
                    {
                        attackerStats.NonNsoTeamkills++;
                    }
                }
                else
                {
                    attackerStats.Kills++;
                    attackerStats.Headshots += death.IsHeadshot ? 1 : 0;

                    attackerStats.ExpGains += death.ExperienceAmount != null ? 1 : 0;
                    attackerStats.ExpAmount += death.ExperienceAmount != null ? (int)death.ExperienceAmount : 0;

                    victimStats.Deaths++;

                    if (attackerStats.FactionId != null && attackerStats.FactionId == 4)
                    {
                        victimStats.NsoDeaths++;
                    }
                    else
                    {
                        victimStats.NonNsoDeaths++;
                    }

                    if (victimStats.FactionId != null && victimStats.FactionId == 4)
                    {
                        attackerStats.NsoKills++;
                    }
                    else
                    {
                        attackerStats.NonNsoKills++;
                    }
                }
            }

            var topPlayers = playersMap.Values.ToList().OrderByDescending(p => p.Kills).Take(take);

            var topPlayerIds = topPlayers.Select(p => p.Id);

            var topPlayerCharacters = await _characterService.GetCharactersById(topPlayerIds);

            var topPlayerOutfitIds = topPlayerCharacters.Where(p => !string.IsNullOrWhiteSpace(p.OutfitId)).Select(p => p.OutfitId).Distinct().ToList();
            var topPlayerOutfitsList = await _outfitService.GetOutfitsByIdsAsync(topPlayerOutfitIds);
            var topPlayerOutfits = topPlayerOutfitsList.ToDictionary(o => o.Id);

            foreach (var player in topPlayers)
            {
                var character = topPlayerCharacters.SingleOrDefault(c => c.Id == player.Id);

                Outfit outfit = null;

                if (character?.OutfitId != null)
                {
                    topPlayerOutfits.TryGetValue(character.OutfitId, out outfit);
                }

                player.Name = character?.Name;
                player.BattleRank = character?.BattleRank;
                player.PrestigeLevel = character?.PrestigeLevel;
                player.OutfitId = character?.OutfitId;
                player.OutfitAlias = outfit?.Alias;
            }

            var leaderboardStats = new ActivityLeaderboardStats
            {
                Players = topPlayers
            };

            return leaderboardStats;
        }

        private static DeathType GetDeathType(DeathWithExperience death)
        {
            var attackerId = death.AttackerCharacterId;
            var victimId = death.VictimCharacterId;

            var attackerFactionId = death.AttackerFactionId;
            var victimFactionId = death.VictimFactionId;

            var isSuicide = (attackerId == victimId || string.IsNullOrWhiteSpace(attackerId) || attackerId == "0");
            if (isSuicide)
            {
                return DeathType.Suicide;
            }

            bool isTeamkill;

            if ((attackerFactionId != null && attackerFactionId.GetValueOrDefault() == 4)
                || (victimFactionId != null && victimFactionId.GetValueOrDefault() == 4))
            {
                isTeamkill = (death.ExperienceAmount == null);
            }
            else
            {
                isTeamkill = (attackerFactionId != null && victimFactionId != null && attackerFactionId == victimFactionId);
            }

            if (isTeamkill)
            {
                return DeathType.Teamkill;
            }

            return DeathType.Kill;
        }
    }
}
