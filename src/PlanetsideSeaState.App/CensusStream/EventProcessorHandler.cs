﻿// Credit to Lampjaw

using Newtonsoft.Json.Linq;
using PlanetsideSeaState.App.CensusStream.EventProcessors;
using PlanetsideSeaState.App.CensusStream.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.CensusStream
{
    public class EventProcessorHandler : IEventProcessorHandler
    {
        private readonly List<EventProcessorDefinition> _processors;

        public EventProcessorHandler(IServiceProvider sp)
        {
            _processors = typeof(IEventProcessor<>).GetTypeInfo().Assembly.GetTypes()
                .Where(a => a.IsClass && !a.IsAbstract)
                .SelectMany(a => a.GetInterfaces())
                .Where(a => a.IsGenericType && typeof(IEventProcessor<>).IsAssignableFrom(a.GetGenericTypeDefinition()))
                .Select(a => CreateEventProcessorDefinition(a, sp.GetService(a)))
                .Where(a => a != null)
                .ToList();
        }

        public async Task<bool> TryProcessAsync(string eventName, JToken payload)
        {
            var processor = _processors.FirstOrDefault(a => a.EventName == eventName);

            if (processor == null)
            {
                return false;
            }

            var inputParam = payload.ToObject(processor.PayloadType, StreamConstants.PayloadDeserializer);

            try
            {
                await (Task)processor.ProcessMethodReference.Invoke(processor.Instance, new[] { inputParam });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing {eventName} payload: {payload} => {ex}");
            }

            return true;
        }

        private static EventProcessorDefinition CreateEventProcessorDefinition(Type serviceType, object instance)
        {
            if (instance == null)
            {
                return null;
            }

            var attr = instance.GetType().GetCustomAttribute<CensusEventProcessorAttribute>();

            if (attr == null)
            {
                return null;
            }

            var payloadType = serviceType.GetGenericArguments().First();
            var processMethodReference = typeof(IEventProcessor<>).MakeGenericType(payloadType).GetMethod("Process", new[] { payloadType });

            return new EventProcessorDefinition(instance, attr.EventName, payloadType, processMethodReference);
        }
    }
}
