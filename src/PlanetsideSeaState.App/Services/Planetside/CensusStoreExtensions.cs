using Microsoft.Extensions.DependencyInjection;

namespace PlanetsideSeaState.App.Services.Planetside
{
    public static class CensusStoreExtensions
    {
        public static IServiceCollection AddCensusStoreServices(this IServiceCollection services)
        {
            services.AddSingleton<ICharacterService, CharacterService>();
            services.AddSingleton<IEventService, EventService>();
            services.AddSingleton<IFacilityService, FacilityService>();
            services.AddSingleton<IOutfitService, OutfitService>();

            return services;
        }
    }
}
