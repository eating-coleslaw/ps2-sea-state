﻿// Credit to Lampjaw

using System;
using System.Reflection;

namespace PlanetsideSeaState.App.CensusStream.Models
{
    public class EventProcessorDefinition
    {
        public EventProcessorDefinition(object instance, string eventName, Type payloadType, MethodInfo processMethodReference)
        {
            Instance = instance;
            EventName = eventName;
            PayloadType = payloadType;
            ProcessMethodReference = processMethodReference;
        }

        public object Instance { get; }
        public string EventName { get; }
        public Type PayloadType { get; }
        public MethodInfo ProcessMethodReference { get; }
    }
}
