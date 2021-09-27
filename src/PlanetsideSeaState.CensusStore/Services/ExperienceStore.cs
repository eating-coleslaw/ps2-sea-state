using PlanetsideSeaState.CensusServices;
using PlanetsideSeaState.CensusServices.Models;
using PlanetsideSeaState.Data.Models.Census;
using PlanetsideSeaState.Data.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PlanetsideSeaState.CensusStore.Services
{
    public class ExperienceStore : IExperienceStore
    {
        public readonly IExperienceRepository _experienceRepository;
        public readonly CensusExperience _censusExperience;

        public string StoreName => "ExperienceStore";

        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public ExperienceStore(IExperienceRepository experienceRepository, CensusExperience censusExperience)
        {
            _experienceRepository = experienceRepository;
            _censusExperience = censusExperience;
        }

        public async Task RefreshStore()
        {
            var experiences = await _censusExperience.GetAllExperiences();

            if (experiences != null)
            {
                await _experienceRepository.UpsertRangeAsync(experiences.Select(ConvertToDbModel));
            }
        }

        private static Experience ConvertToDbModel(CensusExperienceModel model)
        {
            return new Experience
            {
                Id = model.ExperienceId,
                Description = model.Description,
                Xp = model.Xp
            };
        }
    }
}
