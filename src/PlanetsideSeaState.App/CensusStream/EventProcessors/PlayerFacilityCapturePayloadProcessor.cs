using PlanetsideSeaState.Data.Models.Events;
using PlanetsideSeaState.Data.Repositories;
using Microsoft.Extensions.Logging;
using PlanetsideSeaState.App.CensusStream.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.CensusStream.EventProcessors
{
    [CensusEventProcessor("PlayerFacilityCapture")]
    public class PlayerFacilityCapturePayloadProcessor : EventProcessorBase, IEventProcessor<PlayerFacilityCapturePayload>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<PlayerFacilityCapturePayloadProcessor> _logger;

        private readonly PayloadUniquenessFilter<PlayerFacilityCapturePayload> _playerFacilityCaptureFilter = new PayloadUniquenessFilter<PlayerFacilityCapturePayload>();

        private readonly SemaphoreSlim _playerFacilityCaptureSemaphore = new SemaphoreSlim(5);

        public PlayerFacilityCapturePayloadProcessor(IEventRepository eventRepository, ILogger<PlayerFacilityCapturePayloadProcessor> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task Process(PlayerFacilityCapturePayload payload)
        {
            if (!await _playerFacilityCaptureFilter.TryFilterNewPayload(payload, p => $"{p.Timestamp:s}^{p.CharacterId}^{p.FacilityId}"))
            {
                return;
            }

            var characterId = payload.CharacterId;

            if (!IsValidCharacterId(characterId))
            {
                return;
            }

            await _playerFacilityCaptureSemaphore.WaitAsync();

            try
            {
                var dataModel = new PlayerFacilityCapture
                {
                    CharacterId = characterId,
                    Timestamp = payload.Timestamp,
                    WorldId = payload.WorldId,
                    ZoneId = payload.ZoneId.Value,
                    FacilityId = payload.FacilityId,
                    OutfitId = payload.OutfitId == "0" ? null : payload.OutfitId // credit to Lampjaw
                };

                if (ShouldStoreEvent())
                {
                    await _eventRepository.AddAsync(dataModel);
                }


                // Send dataModel to other services for additional handling

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing PlayerFacilityCapture payload: {ex}");
            }
            finally
            {
                _playerFacilityCaptureSemaphore.Release();
            }
        }

        protected override bool ShouldStoreEvent()
        {
            return base.ShouldStoreEvent();
        }
    }
}
