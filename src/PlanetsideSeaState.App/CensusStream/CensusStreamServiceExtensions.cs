using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlanetsideSeaState.App.CensusStream.EventProcessors;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

namespace PlanetsideSeaState.App.CensusStream
{
    public static class CensusStreamServiceExtensions
    {
        public static IServiceCollection AddCensusStreamServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();

            services.Configure<CensusStreamOptions>(configuration);

            services.Configure<CensusStreamOptions>(configuration.GetSection(CensusStreamOptions.CensusStream));


            services.AddTransient<IWebsocketEventHandler, WebsocketEventHandler>();
            
            services.AddTransient<IWebsocketMonitor, WebsocketMonitor>();
            services.AddHostedService<WebsocketMonitorHostedService>();

            services.AddTransient<IWebsocketHealthMonitor, WebsocketHealthMonitor>();

            services.AddEventProcessors();

            services.AddSingleton<IEventProcessorHandler, EventProcessorHandler>();


            return services;
        }

        // Credit to Lampjaw
        private static void AddEventProcessors(this IServiceCollection services)
        {
            var iType = typeof(IEventProcessor<>);
            var types = iType.GetTypeInfo().Assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .SelectMany(t => t.GetInterfaces().Select(i => (t, i)))
                .Where(a => a.i.IsGenericType && iType.IsAssignableFrom(a.i.GetGenericTypeDefinition()))
                .ToList();

            types.ForEach(a => services.AddSingleton(a.i, a.t));
        }
    }
}
