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
            services.AddSingleton<IFactionService, FactionService>();
            services.AddSingleton<IItemService, ItemService>();
            services.AddSingleton<IOutfitService, OutfitService>();
            services.AddSingleton<IProfileService, ProfileService>();
            services.AddSingleton<IVehicleService, VehicleService>();
            services.AddSingleton<IWorldService, WorldService>();
            services.AddSingleton<IZoneService, ZoneService>();

            return services;
        }
    }
}
