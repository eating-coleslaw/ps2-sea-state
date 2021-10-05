using PlanetsideSeaState.App.CensusStream;
using PlanetsideSeaState.App.Services.Planetside;
using PlanetsideSeaState.CensusServices;
using PlanetsideSeaState.CensusStore;
using PlanetsideSeaState.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using PlanetsideSeaState.App.Services.Graphing;

namespace PlanetsideSeaState.App
{
    public static class AppServiceExtensions
    {
        public static IServiceCollection ConfigureAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEntityFrameworkContext(configuration);

            //var serviceKey = configuration["DaybreakGamesServiceKey"];
            var serviceKey = Environment.GetEnvironmentVariable("DaybreakGamesServiceKey", EnvironmentVariableTarget.User);

            services.AddCensusServices(options =>
                options.CensusServiceId = serviceKey);

            services.AddCensusHelpers();
            services.AddCensusStoreServices();
            services.AddCensusStores(configuration);
            services.AddCensusStreamServices(configuration);

            services.AddTransient<IFacilityControlPopulationService, FacilityControlPopulationService>();

            return services;
        }
    }
}
