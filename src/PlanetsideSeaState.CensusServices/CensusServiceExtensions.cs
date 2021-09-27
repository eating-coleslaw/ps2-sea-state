using Microsoft.Extensions.DependencyInjection;

namespace PlanetsideSeaState.CensusServices
{
    public static class CensusServiceExtensions
    {
        public static IServiceCollection AddCensusHelpers(this IServiceCollection services)
        {
            services.AddSingleton<CensusCharacter>();
            services.AddSingleton<CensusExperience>();
            services.AddSingleton<CensusFaction>();
            services.AddSingleton<CensusFacility>();
            services.AddSingleton<CensusItem>();
            services.AddSingleton<CensusItemCategory>();
            services.AddSingleton<CensusLoadout>();
            services.AddSingleton<CensusMetagameEvent>();
            services.AddSingleton<CensusOutfit>();
            services.AddSingleton<CensusProfile>();
            services.AddSingleton<CensusVehicle>();
            services.AddSingleton<CensusWorld>();
            services.AddSingleton<CensusZone>();

            return services;
        }
    }
}
