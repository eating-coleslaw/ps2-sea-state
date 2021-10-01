using PlanetsideSeaState.Data.Models.Events;
using PlanetsideSeaState.Data.Repositories;
using Microsoft.Extensions.Logging;
using PlanetsideSeaState.App.CensusStream.Models;
using System;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.CensusStream.EventProcessors
{
    [CensusEventProcessor("ContinentLock")]
    public class ContinentLockPayloadProcessor : EventProcessorBase, IEventProcessor<ContinentLockPayload>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<ContinentLockPayloadProcessor> _logger;

        private readonly PayloadUniquenessFilter<ContinentLockPayload> _lockFilter = new PayloadUniquenessFilter<ContinentLockPayload>();

        public ContinentLockPayloadProcessor(IEventRepository eventRepository, ILogger<ContinentLockPayloadProcessor> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task Process(ContinentLockPayload payload)
        {
            if (!await _lockFilter.TryFilterNewPayload(payload, p => $"{p.Timestamp:s}"))
            {
                return;
            }

            try
            {
                var dataModel = new ContinentLock
                {
                    Timestamp = payload.Timestamp,
                    WorldId = payload.WorldId,
                    ZoneId = payload.ZoneId.Value,
                    MetagameEventId = payload.MetagameEventId,
                    TriggeringFaction = payload.TriggeringFaction
                };

                if (ShouldStoreEvent())
                {
                    await _eventRepository.AddAsync(dataModel);
                }


                // Send dataModel to other services for additional handling

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing ContinentUnlock payload: {ex}");
            }
        }

        protected override bool ShouldStoreEvent()
        {
            return base.ShouldStoreEvent();
        }
    }
}
