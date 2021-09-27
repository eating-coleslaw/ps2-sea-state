using PlanetsideSeaState.Data.Models.Events;
using PlanetsideSeaState.Data.Repositories;
using Microsoft.Extensions.Logging;
using PlanetsideSeaState.App.CensusStream.Models;
using System;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.CensusStream.EventProcessors
{
    [CensusEventProcessor("GainExperience")]
    public class GainExperiencePayloadProcessor : EventProcessorBase, IEventProcessor<GainExperiencePayload>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<GainExperiencePayloadProcessor> _logger;

        private PayloadUniquenessFilter<GainExperiencePayload> _experienceGainFilter = new PayloadUniquenessFilter<GainExperiencePayload>();

        public GainExperiencePayloadProcessor(IEventRepository eventRepository, ILogger<GainExperiencePayloadProcessor> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task Process(GainExperiencePayload payload)
        {
            if (!await _experienceGainFilter.TryFilterNewPayload(payload, p => $"{p.Timestamp:s}^{p.CharacterId}^{p.ExperienceId}^{p.OtherId}"))
            {
                return;
            }

            var experienceId = payload.ExperienceId;

            var characterId = payload.CharacterId;

            if (!IsValidCharacterId(characterId))
            {
                return;
            }

            try
            {
                var dataModel = new ExperienceGain
                {
                    //Id = Guid.NewGuid(),  // TODO: Is this automatically set on add?
                    Timestamp = payload.Timestamp,
                    CharacterId = characterId,
                    ExperienceId = payload.ExperienceId,
                    Amount = payload.Amount,
                    OtherId = payload.OtherId,
                    LoadoutId = payload.LoadoutId,
                    WorldId = payload.WorldId,
                    ZoneId = payload.ZoneId.Value
                };

                if (ShouldStoreEvent())
                {
                    await _eventRepository.AddAsync(dataModel);
                }


                // Send dataModel to other services for additional handling

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing GainExperience payload: {ex}");

                return;
            }
        }

        protected override bool ShouldStoreEvent()
        {
            return base.ShouldStoreEvent();
        }
    }
}
