using PlanetsideSeaState.Data.Models.Events;
using PlanetsideSeaState.Data.Repositories;
using Microsoft.Extensions.Logging;
using PlanetsideSeaState.App.CensusStream.Models;
using System;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.CensusStream.EventProcessors
{
    [CensusEventProcessor("PlayerLogout")]
    public class PlayerLogoutPayloadProcessor : EventProcessorBase, IEventProcessor<PlayerLogoutPayload>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<PlayerLogoutPayloadProcessor> _logger;

        private readonly PayloadUniquenessFilter<PlayerLogoutPayload> _logoutFilter = new PayloadUniquenessFilter<PlayerLogoutPayload>();

        public PlayerLogoutPayloadProcessor(IEventRepository eventRepository, ILogger<PlayerLogoutPayloadProcessor> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task Process(PlayerLogoutPayload payload)
        {
            if (!await _logoutFilter.TryFilterNewPayload(payload, p => $"{p.Timestamp:s}"))
            {
                return;
            }

            var characterId = payload.CharacterId;

            if (!IsValidCharacterId(characterId))
            {
                return;
            }

            try
            {
                var dataModel = new PlayerLogout
                {
                    CharacterId = characterId,
                    Timestamp = payload.Timestamp,
                    WorldId = payload.WorldId,
                };

                if (ShouldStoreEvent())
                {
                    await _eventRepository.AddAsync(dataModel);
                }


                // Send dataModel to other services for additional handling

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing PlayerLogout payload: {ex}");
            }
        }

        protected override bool ShouldStoreEvent()
        {
            return base.ShouldStoreEvent();
        }
    }
}
