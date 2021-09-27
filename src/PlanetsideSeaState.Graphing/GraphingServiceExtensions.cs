using Microsoft.Extensions.DependencyInjection;
using PlanetsideSeaState.Graphing.Services;

namespace PlanetsideSeaState.Graphing
{
    public static class GraphingServiceExtensions
    {
        public static IServiceCollection AddGraphServices(this IServiceCollection services)
        {
            services.AddTransient<FacilityPopGraphService>();
            services.AddSingleton<IFacilityPopGraphServicesManager, FacilityPopGraphServicesManager>();
            services.AddSingleton<IFacilityPopGraphServiceHelper, FacilityPopGraphServiceHelper>();

            services.AddHostedService<FacilityPopGraphServicesManagerHostedService>();

            return services;
        }
    }
}
