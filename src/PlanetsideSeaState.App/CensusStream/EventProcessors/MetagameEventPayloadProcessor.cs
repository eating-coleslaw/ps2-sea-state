using PlanetsideSeaState.Data.Models.Events;
using PlanetsideSeaState.Data.Repositories;
using Microsoft.Extensions.Logging;
using PlanetsideSeaState.App.CensusStream.Models;
using System;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.CensusStream.EventProcessors
{
    [CensusEventProcessor("MetagameEvent")]
    public class MetagameEventPayloadProcessor : EventProcessorBase, IEventProcessor<MetagameEventPayload>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<MetagameEventPayloadProcessor> _logger;
        
        private readonly PayloadUniquenessFilter<MetagameEventPayload> _metagameFilter = new PayloadUniquenessFilter<MetagameEventPayload>();

        public MetagameEventPayloadProcessor(IEventRepository eventRepository, ILogger<MetagameEventPayloadProcessor> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task Process(MetagameEventPayload payload)
        {
            if (!await _metagameFilter.TryFilterNewPayload(payload, p => p.Timestamp.ToString("s")))
            {
                return;
            }

            try
            {
                var dataModel = new MetagameEvent
                {
                    InstanceId = payload.InstanceId,
                    MetagameEventId = payload.MetagameEventId,
                    MetagameEventState = payload.MetagameEventState,
                    ZoneControlVs = payload.FactionVs,
                    ZoneControlNc = payload.FactionNc,
                    ZoneControlTr = payload.FactionTr,
                    ExperienceBonus = (int)payload.ExperienceBonus,
                    Timestamp = payload.Timestamp,
                    WorldId = payload.WorldId,
                    ZoneId = payload.ZoneId
                };

                if (ShouldStoreEvent())
                {
                    await _eventRepository.AddAsync(dataModel);
                }


                // Send dataModel to other services for additional handling

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing MetagameEvent payload: {ex}");
            }
        }

        protected override bool ShouldStoreEvent()
        {
            return base.ShouldStoreEvent();
        }
    }
}
