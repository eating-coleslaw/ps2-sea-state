using PlanetsideSeaState.Data.Models.Events;
using PlanetsideSeaState.Data.Repositories;
using Microsoft.Extensions.Logging;
using PlanetsideSeaState.App.CensusStream.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.CensusStream.EventProcessors
{
    [CensusEventProcessor("ContinentUnlock")]
    public class ContinentUnlockPayloadProcessor : EventProcessorBase, IEventProcessor<ContinentUnlockPayload>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<ContinentUnlockPayloadProcessor> _logger;

        private readonly PayloadUniquenessFilter<ContinentUnlockPayload> _unlockFilter = new PayloadUniquenessFilter<ContinentUnlockPayload>();

        private readonly SemaphoreSlim _continentUnlockSemaphore = new SemaphoreSlim(1);

        public ContinentUnlockPayloadProcessor(IEventRepository eventRepository, ILogger<ContinentUnlockPayloadProcessor> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task Process(ContinentUnlockPayload payload)
        {
            if (!await _unlockFilter.TryFilterNewPayload(payload, p => $"{p.Timestamp:s}^{p.WorldId}^{p.ZoneId}"))
            {
                return;
            }

            await _continentUnlockSemaphore.WaitAsync();

            try
            {
                var dataModel = new ContinentUnlock
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
            finally
            {
                _continentUnlockSemaphore.Release();
            }
        }

        protected override bool ShouldStoreEvent()
        {
            return base.ShouldStoreEvent();
        }
    }
}
