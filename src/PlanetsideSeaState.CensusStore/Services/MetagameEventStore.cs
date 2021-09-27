using PlanetsideSeaState.CensusServices;
using PlanetsideSeaState.CensusServices.Models;
using PlanetsideSeaState.Data.Models.Census;
using PlanetsideSeaState.Data.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PlanetsideSeaState.CensusStore.Services
{
    public class MetagameEventStore : IMetagameEventStore
    {
        public readonly IMetagameEventRepository _metagameEventRepository;
        public readonly CensusMetagameEvent _censusMetagameEvent;

        public string StoreName => "MetagameEventStore";

        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public MetagameEventStore(IMetagameEventRepository metagameEventRepository, CensusMetagameEvent censusMetagameEvent)
        {
            _metagameEventRepository = metagameEventRepository;
            _censusMetagameEvent = censusMetagameEvent;
        }

        public async Task<MetagameEventCategory> GetMetagameEventCategoryAsync(int metagameEventId)
        {
            return await _metagameEventRepository.GetMetagameEventAsync(metagameEventId);
        }
        
        public async Task RefreshStore()
        {
            var categories = await _censusMetagameEvent.GetAllMetagameEvents();
            var states = await _censusMetagameEvent.GetAllMetagameEventStates();

            if (categories != null)
            {
                await _metagameEventRepository.UpsertRangeAsync(categories.Select(ConvertToDbModel));
            }

            if (states != null)
            {
                await _metagameEventRepository.UpsertRangeAsync(states.Select(ConvertToDbModel));
            }
        }

        private static MetagameEventCategory ConvertToDbModel(CensusMetagameEventModel model)
        {
            return new MetagameEventCategory
            {
                Id = model.MetagameEventId,
                Name = model.Name?.English,
                Description = model.Description?.English,
                ExperienceBonus = model.ExperienceBonus,
                Type = model.Type
            };
        }

        private static MetagameEventState ConvertToDbModel(CensusMetagameEventStateModel model)
        {
            return new MetagameEventState
            {
                Id = model.MetagameEventStateId,
                Name = model.Name
            };
        }
    }
}
