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

        private readonly SemaphoreSlim _playerFacilityCaptureSemaphore = new(5);

        public PlayerFacilityCapturePayloadProcessor(IEventRepository eventRepository, ILogger<PlayerFacilityCapturePayloadProcessor> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task Process(PlayerFacilityCapturePayload payload)
        {
            if (!await _playerFacilityCaptureFilter.TryFilterNewPayload(payload, p => $"{p.Timestamp:s}"))
            {
                return;
            }

            if (!IsValidCharacterId(payload.CharacterId))
            {
                return;
            }

            await _playerFacilityCaptureSemaphore.WaitAsync();

            try
            {
                var dataModel = new PlayerFacilityControl
                {
                    FacilityId = payload.FacilityId,
                    Timestamp = payload.Timestamp,
                    CharacterId = payload.CharacterId,
                    IsCapture = true,
                    WorldId = payload.WorldId,
                    ZoneId = payload.ZoneId ?? 0,
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
