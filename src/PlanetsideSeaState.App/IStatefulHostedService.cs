﻿// Credit to Lampjaw

using PlanetsideSeaState.App.Models;
using System.Threading;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App
{
    public interface IStatefulHostedService
    {
        Task OnApplicationStartup(CancellationToken cancellationToken);
        Task OnApplicationShutdown(CancellationToken cancellationToken);
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
        Task<ServiceState> GetStateAsync(CancellationToken cancellationToken);
        string ServiceName { get; }
    }
}
