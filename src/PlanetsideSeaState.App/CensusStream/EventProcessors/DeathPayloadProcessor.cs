using PlanetsideSeaState.Data.Models.Events;
using PlanetsideSeaState.Data.Repositories;
using Microsoft.Extensions.Logging;
using PlanetsideSeaState.App.CensusStream.Models;
using System;
using System.Threading.Tasks;
using PlanetsideSeaState.App.Services.Planetside;
using PlanetsideSeaState.Data.Models.Census;

namespace PlanetsideSeaState.App.CensusStream.EventProcessors
{
    [CensusEventProcessor("Death")]
    public class DeathPayloadProcessor : EventProcessorBase, IEventProcessor<DeathPayload>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICharacterService _characterService;
        private readonly ILogger<DeathPayloadProcessor> _logger;


        private readonly PayloadUniquenessFilter<DeathPayload> _deathFilter = new PayloadUniquenessFilter<DeathPayload>();

        public DeathPayloadProcessor(IEventRepository eventRepository, ICharacterService characterService, ILogger<DeathPayloadProcessor> logger)
        {
            _eventRepository = eventRepository;
            _characterService = characterService;
            _logger = logger;
        }

        public async Task Process(DeathPayload payload)
        {
            if (!await _deathFilter.TryFilterNewPayload(payload, p => $"{p.Timestamp:s}"))
            {
                return;
            }

            string attackerId = payload.AttackerCharacterId;
            string victimId = payload.CharacterId;

            bool isValidAttackerId = IsValidCharacterId(attackerId);
            bool isValidVictimId = IsValidCharacterId(victimId);

            try
            {
                Character attackerCharacter = null;
                Character victimCharacter = null;
                
                if (isValidAttackerId == true)
                {
                    // Do stuff like look up the Character name, faction, etc.
                    attackerCharacter = await _characterService.GetCharacter(attackerId);
                }

                if (isValidVictimId == true)
                {
                    // Do stuff like look up the Character name, faction, etc.
                    victimCharacter = await _characterService.GetCharacter(victimId);
                }

                var dataModel = new Death
                {
                    Timestamp = payload.Timestamp,
                    AttackerCharacterId = attackerId,
                    VictimCharacterId = victimId,
                    ZoneId = payload.ZoneId,
                    WorldId = payload.WorldId,
                    AttackerLoadoutId = payload.AttackerLoadoutId,
                    VictimLoadoutId = payload.CharacterLoadoutId,
                    AttackerFactionId = attackerCharacter?.FactionId,
                    VictimFactionId = victimCharacter?.FactionId,
                    WeaponId = payload.AttackerWeaponId,
                    AttackerVehicleId = payload.AttackerVehicleId,
                    IsHeadshot = payload.IsHeadshot
                };

                if (ShouldStoreEvent())
                {
                    await _eventRepository.AddAsync(dataModel);
                }


                // Send dataModel to other services for additional handling

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing Death payload: {ex}");

                return;
            }
        }

        protected override bool ShouldStoreEvent()
        {
            return base.ShouldStoreEvent();
        }
    }
}
