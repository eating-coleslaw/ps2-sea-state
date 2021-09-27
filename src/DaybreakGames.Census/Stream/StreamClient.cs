// Credit to Lampjaw

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Websocket.Client;

namespace DaybreakGames.Census.Stream
{
    public class StreamClient : IStreamClient
    {
        private readonly IOptions<CensusOptions> _options;


        private const string CensusWebsocketEndpoint = "wss://push.planetside2.com/streaming";
        private const string CensusServiceNamespace = "ps2";

        private readonly string CensusServiceKey;

        private static readonly Func<ClientWebSocket> wsFactory = new Func<ClientWebSocket>(() =>
        {
            return new ClientWebSocket { Options = { KeepAliveInterval = TimeSpan.FromSeconds(5) } };
        });

        private static readonly JsonSerializerSettings sendMessageSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private readonly ILogger<StreamClient> _logger;


        public StreamClient(IOptions<CensusOptions> options, ILogger<StreamClient> logger)
        {
            _options = options;
            _logger = logger;

            CensusServiceKey = _options.Value.CensusServiceId;
        }

        private Func<string, Task> _onMessage;
        private Func<DisconnectionInfo, Task> _onDisconnected;
        private Func<ReconnectionType, Task> _onConnect;

        private IWebsocketClient _client;

        public StreamClient OnConnect(Func<ReconnectionType, Task> onConnect)
        {
            _onConnect = onConnect;
            return this;
        }

        public StreamClient OnDisconnect(Func<DisconnectionInfo, Task> onDisconnect)
        {
            _onDisconnected = onDisconnect;
            return this;
        }

        public StreamClient OnMessage(Func<string, Task> onMessage)
        {
            _onMessage = onMessage;
            return this;
        }

        public async Task ConnectAsync()
        {
            _client = new WebsocketClient(GetEndpoint(), wsFactory)
            {
                ReconnectTimeout = TimeSpan.FromSeconds(35),
                ErrorReconnectTimeout = TimeSpan.FromSeconds(30)
            };

            Console.WriteLine($"Connection URI: {GetEndpoint()}");

            _client.DisconnectionHappened.Subscribe(info =>
            {
                _logger.LogWarning(75421, $"Stream disconnected: {info.Type}: {info.Exception}");

                if (_onDisconnected != null)
                {
                    Task.Run(() => _onDisconnected(info));
                }
            });

            _client.ReconnectionHappened.Subscribe(info =>
            {
                if (info.Type == ReconnectionType.Initial)
                {
                    _logger.LogInformation("Starting initial census stream connect");
                }
                else
                {

                    _logger.LogInformation($"Stream reconnection occured: {info.Type}");
                }

                if (_onConnect != null)
                {
                    Task.Run(() => _onConnect(info.Type));
                }
            });

            _client.MessageReceived.Subscribe(msg =>
            {
                if (_onMessage != null)
                {
                    Task.Run(() => _onMessage(msg.Text));
                }
            });

            await _client.Start();
        }

        public void Subscribe(CensusStreamSubscription subscription)
        {
            var sMessage = JsonConvert.SerializeObject(subscription, sendMessageSettings);

            _logger.LogInformation($"Subscribing to census with: {sMessage}");

            _client.Send(sMessage);
        }

        public Task DisconnectAsync()
        {
            _client?.Dispose();
            return Task.CompletedTask;
        }

        public Task ReconnectAsync()
        {
            return _client?.Reconnect();
        }

        private Uri GetEndpoint()
        {
            return new Uri($"{CensusWebsocketEndpoint}?environment={CensusServiceNamespace}&service-id=s:{CensusServiceKey}");
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
