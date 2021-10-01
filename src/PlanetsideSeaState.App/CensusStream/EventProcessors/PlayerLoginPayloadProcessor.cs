using PlanetsideSeaState.Data.Models.Events;
using PlanetsideSeaState.Data.Repositories;
using Microsoft.Extensions.Logging;
using PlanetsideSeaState.App.CensusStream.Models;
using System;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.CensusStream.EventProcessors
{
    [CensusEventProcessor("PlayerLogin")]
    public class PlayerLoginPayloadProcessor : EventProcessorBase, IEventProcessor<PlayerLoginPayload>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<PlayerLoginPayloadProcessor> _logger;

        private readonly PayloadUniquenessFilter<PlayerLoginPayload> _loginFilter = new PayloadUniquenessFilter<PlayerLoginPayload>();

        public PlayerLoginPayloadProcessor(IEventRepository eventRepository, ILogger<PlayerLoginPayloadProcessor> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task Process(PlayerLoginPayload payload)
        {
            if (!await _loginFilter.TryFilterNewPayload(payload, p => $"{p.Timestamp:s}"))
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
                var dataModel = new PlayerLogin
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
                _logger.LogError($"Error processing PlayerLogin payload: {ex}");
            }
        }

        protected override bool ShouldStoreEvent()
        {
            return base.ShouldStoreEvent();
        }
    }
}
