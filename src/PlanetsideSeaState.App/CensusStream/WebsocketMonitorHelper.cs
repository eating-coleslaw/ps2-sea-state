using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.CensusStream
{
    public class WebsocketMonitorHelper : IWebsocketMonitorHelper
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public WebsocketMonitorHelper(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public WebsocketMonitorFactory GetFactory()
        {
            return new WebsocketMonitorFactory(_scopeFactory);
        }

        public class WebsocketMonitorFactory : IAsyncDisposable
        {
            private readonly IServiceScope _scope;
            private readonly WebsocketMonitor _wsMonitor;

            public WebsocketMonitorFactory(IServiceScopeFactory scopeFactory)
            {
                _scope = scopeFactory.CreateScope();
                _wsMonitor = _scope.ServiceProvider.GetRequiredService<WebsocketMonitor>();
            }

            public WebsocketMonitor GetWsMonitor()
            {
                return _wsMonitor;
            }

            public async Task<WebsocketMonitor> GetAndStartWsMonitor()
            {
                await _wsMonitor.StartAsync(CancellationToken.None);
                return _wsMonitor;
            }

            public async ValueTask DisposeAsync()
            {
                await _wsMonitor.StopAsync(CancellationToken.None);
                _wsMonitor.Dispose();
                _scope.Dispose();
            }
        }
    }
}
