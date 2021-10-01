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

        private readonly SemaphoreSlim _playerFacilityDefendSemaphore = new(5);

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

            if (!IsValidCharacterId(payload.CharacterId))
            {
                return;
            }

            await _playerFacilityDefendSemaphore.WaitAsync();

            try
            {
                var dataModel = new PlayerFacilityControl
                {
                    FacilityId = payload.FacilityId,
                    Timestamp = payload.Timestamp,
                    CharacterId = payload.CharacterId,
                    IsCapture = false,
                    WorldId = payload.WorldId,
                    ZoneId = payload.ZoneId.Value,
                    OutfitId = payload.OutfitId == "0" ? null : payload.OutfitId
                };

                if (ShouldStoreEvent())
                {
                    new Thread(async () =>
                    {
                        // Wait a second for the corresponding FacilityControl event to be received
                        await Task.Delay(1000);

                        var facilityControl = await _eventRepository.GetFacilityControl(dataModel.FacilityId, dataModel.Timestamp, dataModel.WorldId);

                        if (facilityControl == null)
                        {
                            return;
                        }

                        dataModel.FacilityControlId = facilityControl.Id;

                        await _eventRepository.AddAsync(dataModel);

                    }).Start();
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
