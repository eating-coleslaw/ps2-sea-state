using PlanetsideSeaState.Data.Models.Events;
using PlanetsideSeaState.Data.Repositories;
using Microsoft.Extensions.Logging;
using PlanetsideSeaState.App.CensusStream.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.CensusStream.EventProcessors
{
    [CensusEventProcessor("PlayerFacilityDefend")]
    public class PlayerFacilityDefendPayloadProcessor : EventProcessorBase, IEventProcessor<PlayerFacilityDefendPayload>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<PlayerFacilityDefendPayloadProcessor> _logger;

        private readonly PayloadUniquenessFilter<PlayerFacilityDefendPayload> _playerFacilityDefendFilter = new PayloadUniquenessFilter<PlayerFacilityDefendPayload>();

        private readonly SemaphoreSlim _playerFacilityDefendSemaphore = new SemaphoreSlim(5);

        public PlayerFacilityDefendPayloadProcessor(IEventRepository eventRepository, ILogger<PlayerFacilityDefendPayloadProcessor> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task Process(PlayerFacilityDefendPayload payload)
        {
            if (!await _playerFacilityDefendFilter.TryFilterNewPayload(payload, p => $"{p.Timestamp:s}"))
            {
                return;
            }

            var characterId = payload.CharacterId;

            if (!IsValidCharacterId(characterId))
            {
                return;
            }

            await _playerFacilityDefendSemaphore.WaitAsync();

            try
            {
                var dataModel = new PlayerFacilityDefend
                {
                    CharacterId = characterId,
                    Timestamp = payload.Timestamp,
                    WorldId = payload.WorldId,
                    ZoneId = payload.ZoneId.Value,
                    FacilityId = payload.FacilityId,
                    OutfitId = payload.OutfitId == "0" ? null : payload.OutfitId
                };

                if (ShouldStoreEvent())
                {
                    await _eventRepository.AddAsync(dataModel);
                }


                // Send dataModel to other services for additional handling

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing PlayerFacilityDefend payload: {ex}");
            }
            finally
            {
                _playerFacilityDefendSemaphore.Release();
            }
        }

        protected override bool ShouldStoreEvent()
        {
            return base.ShouldStoreEvent();
        }
    }
}
