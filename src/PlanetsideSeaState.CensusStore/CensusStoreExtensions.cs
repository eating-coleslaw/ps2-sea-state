using PlanetsideSeaState.CensusServices;
using PlanetsideSeaState.CensusStore.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PlanetsideSeaState.CensusStore
{
    public static class CensusStoreExtensions
    {
        public static IServiceCollection AddCensusStores(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<StoreOptions>(configuration);

            services.AddCensusHelpers();

            services.AddSingleton<ICharacterStore, CharacterStore>();
            services.AddTransient<IExperienceStore, ExperienceStore>();
            services.AddSingleton<IFacilityStore, FacilityStore>();
            services.AddSingleton<IOutfitStore, OutfitStore>();

            services.AddHostedService<StoreUpdaterSchedulerHostedService>();

            return services;
        }
    }
}
