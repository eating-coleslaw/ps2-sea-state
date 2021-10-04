// Credit to Lampjaw @ Voidwell.DaybreakGames

using DaybreakGames.Census.Stream;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using PlanetsideSeaState.App.CensusStream.Models;
using PlanetsideSeaState.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Websocket.Client;

namespace PlanetsideSeaState.App.CensusStream
{
    public class WebsocketMonitor : StatefulHostedService, IWebsocketMonitor, IDisposable
    {
        private readonly IStreamClient _client;
        private readonly IWebsocketHealthMonitor _healthMonitor;
        private readonly IWebsocketEventHandler _handler;
        private readonly CensusStreamOptions _options;
        private readonly ILogger<WebsocketMonitor> _logger;

        private StreamState _lastStateChange;
        private Timer _timer;

        public override string ServiceName => "CensusMonitor";

        public List<string> CharacterSubscriptions = new List<string>();

        public int? SubscribedFacilityId;
        public int? SubscribedWorldId;

        private bool disposedValue;

        public WebsocketMonitor(IStreamClient client, IWebsocketHealthMonitor healthMonitor, IWebsocketEventHandler handler,
            IOptions<CensusStreamOptions> options, ILogger<WebsocketMonitor> logger)
        {
            _client = client;
            _healthMonitor = healthMonitor;
            _handler = handler;
            _options = options.Value;
            _logger = logger;

            _client.OnConnect(OnConnect)
                   .OnMessage(OnMessage)
                   .OnDisconnect(OnDisconnect);
        }
        
        private CensusStreamSubscription CreateSubscription()
        {
            var eventNames = new List<string>();

            if (_options.Services != null)
            {
                eventNames.AddRange(_options.Services);
            }

            if (_options.ExperienceIds != null && _options.ExperienceIds.Any())
            {
                var experienceEvents = _options.ExperienceIds.Select(id => $"GainExperience_experience_id_{id}");
                eventNames.AddRange(experienceEvents);
            }

            //Console.WriteLine("============= Websocket Monitor =============");

            //Console.WriteLine($"Characters: {_options.Characters?.Count()}");
            //Console.WriteLine($"Worlds: {_options.Worlds?.Count()}");

            return new CensusStreamSubscription
            {
                Characters = _options.Characters,
                Worlds = _options.Worlds,
                LogicalAndCharactersWithWorlds = _options.LogicalAndCharactersWithWorlds,
                EventNames = eventNames
            };
        }

        public override async Task StartInternalAsync(CancellationToken cancellationToken)
        {
            if (_options.DisableWebsocketMonitor)
            {
                _logger.LogInformation("Census stream monitor is disabled");
                return;
            }

            _logger.LogInformation("Starting census stream monitor");

            await _client.ConnectAsync();

            _timer = new Timer(CheckDataHealth, null, 0, (int)TimeSpan.FromMinutes(1).TotalMilliseconds);
        }

        public override async Task StopInternalAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping census stream monitor");

            if (_client == null)
            {
                return;
            }

            await _client?.DisconnectAsync();
            _timer?.Dispose();
        }

        public override async Task OnApplicationShutdown(CancellationToken cancellationToken)
        {
            await _client?.DisconnectAsync();
        }

        protected override Task<object> GetStatusAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((object)_lastStateChange);
        }

        private Task OnConnect(ReconnectionType type)
        {
            _client.Subscribe(CreateSubscription());
            return Task.CompletedTask;
        }

        private Task OnMessage(string message)
        {
            if (message == null)
            {
                return Task.CompletedTask;
            }

            JToken jMsg;

            try
            {
                jMsg = JToken.Parse(message);
            }
            catch (Exception)
            {
                _logger.LogError(91097, "Failed to parse message: {0}", message);
                return Task.CompletedTask;
            }

            if (jMsg.Value<string>("type") == "heartbeat")
            {
                UpdateStateDetails(jMsg.ToObject<object>());
                return Task.CompletedTask;
            }

            Task.Run(() =>
            {
                _handler.Process(jMsg);

            });

            return Task.CompletedTask;
        }

        private Task OnDisconnect(DisconnectionInfo info)
        {
            UpdateStateDetails(info.Exception?.Message ?? info.Type.ToString());
            _healthMonitor.ClearAllWorlds();
            return Task.CompletedTask;
        }

        private void UpdateStateDetails(object contents)
        {
            _lastStateChange = new StreamState
            {
                LastStateChangeTime = DateTime.UtcNow,
                Contents = contents
            };
        }

        private async void CheckDataHealth(object state)
        {
            if (!IsRunning)
            {
                _healthMonitor.ClearAllWorlds();
                _timer?.Dispose();
                return;
            }

            if (!_healthMonitor.IsHealthy())
            {
                _logger.LogError(45234, "Census stream has failed health checks. Attempting resetting connection.");

                try
                {
                    await _client?.ReconnectAsync();
                }
                catch (Exception ex)
                {
                    UpdateStateDetails(ex.Message);

                    _logger.LogError(45235, ex, "Failed to reestablish connection to Census");
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _client?.Dispose();
                    _timer?.Dispose();

                    CharacterSubscriptions.Clear();
                    CharacterSubscriptions = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}
