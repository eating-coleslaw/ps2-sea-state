using PlanetsideSeaState.Data.Models.Events;
using PlanetsideSeaState.Data.Repositories;
using Microsoft.Extensions.Logging;
using PlanetsideSeaState.Shared.Planetside;
using PlanetsideSeaState.App.CensusStream.Models;
using System;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.CensusStream.EventProcessors
{
    [CensusEventProcessor("FacilityControl")]
    public class FacilityControlPayloadProcessor : EventProcessorBase, IEventProcessor<FacilityControlPayload>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<FacilityControlPayloadProcessor> _logger;

        private PayloadUniquenessFilter<FacilityControlPayload> _facilityControlFilter = new PayloadUniquenessFilter<FacilityControlPayload>();

        public FacilityControlPayloadProcessor(IEventRepository eventRepository, ILogger<FacilityControlPayloadProcessor> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task Process(FacilityControlPayload payload)
        {
            if (!await _facilityControlFilter.TryFilterNewPayload(payload, p => $"{p.Timestamp:s}"))
            {
                return;
            }

            var oldFactionId = payload.OldFactionId;
            var newFactionId = payload.NewFactionId;

            try
            {

                var controlType = GetFacilityControlType(oldFactionId, newFactionId);

                if (!controlType.HasValue)
                {
                    return;
                }

                var dataModel = new FacilityControl
                {
                    Timestamp = payload.Timestamp,
                    ControlType = controlType.Value,
                    NewFactionId = payload.NewFactionId,
                    OldFactionId = payload.OldFactionId,
                    FacilityId = payload.FacilityId,
                    ZoneId = payload.ZoneId,
                    WorldId = payload.WorldId
                };


                if (ShouldStoreEvent())
                {
                    await _eventRepository.AddAsync(dataModel);
                }


                // Send dataModel to other services for additional handling

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing FacilityControl payload: {ex}");

                return;
            }
        }

        private static FacilityControlType? GetFacilityControlType(int? oldFactionId, int? newFactionId)
        {
            if (oldFactionId != null && (int)oldFactionId < 0)
            {
                throw new ArgumentException($"{oldFactionId} is not a valid value for oldFactionId");
            }

            if (newFactionId != null && (int)newFactionId < 0)
            {
                throw new ArgumentException($"{newFactionId} is not a valid value for newFactionId");
            }

            if (newFactionId == null || newFactionId == 0)
            {
                return null;
            }
            else if (oldFactionId == null || oldFactionId == 0)
            {
                return FacilityControlType.Capture;
            }
            else
            {
                return oldFactionId == newFactionId
                            ? FacilityControlType.Defense
                            : FacilityControlType.Capture;
            }
        }

        protected override bool ShouldStoreEvent()
        {
            return base.ShouldStoreEvent();
        }

    }
}
