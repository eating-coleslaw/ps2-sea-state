using System.Threading;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Shared
{
    public abstract class StatefulService : IStatefulService
    {
        protected bool IsRunning { get; set; } = false;
        public abstract string ServiceName { get; }

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            await UpdateStateAsync(true);
            await StartInternalAsync(cancellationToken);
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            await UpdateStateAsync(false);
            await StopInternalAsync(cancellationToken);
        }

        public async Task<ServiceState> GetStateAsync(CancellationToken cancellationToken)
        {
            var details = await GetStatusAsync(cancellationToken);

            return new ServiceState
            {
                Name = ServiceName,
                IsEnabled = IsRunning,
                Details = details
            };
        }

        protected async Task UpdateStateAsync(bool isEnabled)
        {
            IsRunning = isEnabled;
            var state = await GetStateAsync(CancellationToken.None);
        }

        protected virtual Task<object> GetStatusAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(null as object);
        }

        public abstract Task StartInternalAsync(CancellationToken cancellationToken);
        public abstract Task StopInternalAsync(CancellationToken cancellationToken);
    }
}
