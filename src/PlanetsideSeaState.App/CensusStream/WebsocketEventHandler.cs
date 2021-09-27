using DaybreakGames.Census;
using DaybreakGames.Census.JsonConverters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlanetsideSeaState.App.CensusStream.Models;
using System;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.CensusStream
{
    public class WebsocketEventHandler : IWebsocketEventHandler
    {
        private readonly IEventProcessorHandler _processorHandler;
        private readonly IWebsocketHealthMonitor _healthMonitor;
        private readonly ILogger<WebsocketEventHandler> _logger;

        // Credit to Voidwell @Lampjaw
        private readonly JsonSerializer _payloadDeserializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            ContractResolver = new UnderscorePropertyNamesContractResolver(),
            Converters = new JsonConverter[]
                {
                    new BooleanJsonConverter(),
                    new DateTimeJsonConverter()
                }
        });

        public WebsocketEventHandler(IEventProcessorHandler processorHandler, IWebsocketHealthMonitor healthMonitor, ILogger<WebsocketEventHandler> logger)
        {
            _processorHandler = processorHandler;
            _healthMonitor = healthMonitor;
            _logger = logger;
        }

        public async Task Process(JToken message)
        {
            await ProcessServiceEvent(message);
        }

        // Credit to Voidwell @Lampjaw
        private async Task ProcessServiceEvent(JToken message)
        {
            var jPayload = message.SelectToken("payload");

            var payload = jPayload?.ToObject<PayloadBase>(_payloadDeserializer);
            var eventName = payload?.EventName;

            if (eventName == null)
            {
                return;
            }

            _healthMonitor.ReceivedEvent(payload.WorldId, eventName);

            try
            {
                if (!await _processorHandler.TryProcessAsync(eventName, jPayload))
                {
                    _logger.LogWarning("No process method found for event: {0}", eventName);
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(75642, ex, "Failed to process websocket event: {0}. {1}", eventName, jPayload.ToString());
            }
        }
    }
}
