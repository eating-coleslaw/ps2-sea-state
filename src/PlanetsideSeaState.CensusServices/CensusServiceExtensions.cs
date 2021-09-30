using Microsoft.Extensions.DependencyInjection;

namespace PlanetsideSeaState.CensusServices
{
    public static class CensusServiceExtensions
    {
        public static IServiceCollection AddCensusHelpers(this IServiceCollection services)
        {
            services.AddSingleton<CensusCharacter>();
            services.AddSingleton<CensusExperience>();
            services.AddSingleton<CensusFacility>();
            services.AddSingleton<CensusOutfit>();

            return services;
        }
    }
}
