﻿// Credit to Lampjaw

using PlanetsideSeaState.Data.Models;
using PlanetsideSeaState.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace PlanetsideSeaState.CensusStore
{
    public class StoreUpdaterSchedulerHostedService : IHostedService
    {
        private readonly IUpdaterSchedulerRepository _updaterSchedulerRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly StoreOptions _options;
        private readonly ILogger<StoreUpdaterSchedulerHostedService> _logger;
        private readonly Dictionary<string, Timer> _updaterTimers = new();

        private readonly List<object> _pendingWork = new();
        private bool _isWorking = false;

        public StoreUpdaterSchedulerHostedService(IUpdaterSchedulerRepository updaterSchedulerRepository, IServiceProvider serviceProvider,
            IOptions<StoreOptions> options, ILogger<StoreUpdaterSchedulerHostedService> logger)
        {
            _updaterSchedulerRepository = updaterSchedulerRepository;
            _serviceProvider = serviceProvider;
            _options = options.Value;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_options.DisableUpdater)
            {
                return Task.CompletedTask;
            }

            var updatableTypes = typeof(IUpdateable).GetTypeInfo().Assembly.GetTypes()
                .Where(a => typeof(IUpdateable).IsAssignableFrom(a));

            var storeUpdaterInterfaces = updatableTypes.Where(a => a.GetTypeInfo().IsInterface && !typeof(IUpdateable).IsEquivalentTo(a));
            var storeUpdaterTypes = updatableTypes.Where(a => a.GetTypeInfo().IsClass && !a.GetTypeInfo().IsAbstract);

            var storeUpdaterMatches = storeUpdaterTypes.Select(t => new[] { t, storeUpdaterInterfaces.SingleOrDefault(i => i.IsAssignableFrom(t)) })
                .Where(m => m[1] != null);

            foreach (var updaterPair in storeUpdaterMatches)
            {
                RegisterUpdater(updaterPair);
            }

            _logger.LogInformation("Updater scheduler ready.");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Updater scheduler stopped!");

            _updaterTimers.Clear();

            return Task.CompletedTask;
        }

        private void RegisterUpdater(Type[] updaterPair)
        {
            var updater = _serviceProvider.GetRequiredService(updaterPair[1]) as IUpdateable;
            UpdaterScheduler updaterHistory = null;

            try
            {
                updaterHistory = _updaterSchedulerRepository.GetUpdaterHistoryByServiceName(updater.StoreName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failed get updated history: {ex}");
            }
            //var updaterHistory = _updaterSchedulerRepository.GetUpdaterHistoryByServiceName(updater.StoreName);

            if (_updaterTimers.ContainsKey(updater.StoreName))
                return;

            var remainingInterval = TimeSpan.Zero;

            if (updaterHistory?.LastUpdateDate != null)
            {
                var offset = updaterHistory.LastUpdateDate.Add(updater.UpdateInterval) - DateTime.UtcNow;
                if (offset.TotalMilliseconds > 0)
                {
                    remainingInterval = offset;
                }
            }

            var timer = new Timer(HandleTimer, updaterPair, remainingInterval, updater.UpdateInterval);
            _updaterTimers.Add(updater.StoreName, timer);
        }

        private async void HandleTimer(object stateInfo)
        {
            if (_isWorking)
            {
                _pendingWork.Add(stateInfo);
                return;
            }

            _isWorking = true;

            var updaterPair = stateInfo as Type[];

            var updaterService = _serviceProvider.GetRequiredService(updaterPair[1]) as IUpdateable;

            _logger.LogInformation($"Updating {updaterService.StoreName}.");

            try
            {
                await updaterService.RefreshStore();

                _logger.LogInformation($"Update complete for {updaterService.StoreName}.");

                var dataModel = new UpdaterScheduler { Id = updaterService.StoreName, LastUpdateDate = DateTime.UtcNow };

                await _updaterSchedulerRepository.UpsertAsync(dataModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Update failed for {updaterService.StoreName}: {ex}");
            }
            finally
            {
                _isWorking = false;

                if (_pendingWork.Count > 0)
                {
                    var pendingWork = _pendingWork[0];
                    _pendingWork.RemoveAt(0);
                    HandleTimer(pendingWork);
                }
            }
        }
    }
}
