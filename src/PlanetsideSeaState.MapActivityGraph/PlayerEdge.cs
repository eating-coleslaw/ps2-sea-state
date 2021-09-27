using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlanetsideSeaState.MapActivityGraph
{
    public class PlayerEdge
    {
        public readonly PlayerNode Parent;
        public readonly PlayerNode Child;

        public DateTime Timestamp { get; set; }

        // Max time, in milliseconds, before the edge expires
        public int Lifetime { get; private set; }
        private Timer ExpirationTimer { get; set; }
        public event EventHandler<EdgeExpirationEventArgs<PlayerEdge>> ExpirationReached;

        private bool _isExpiring = false;


        public PlayerEdge(PlayerNode parent, PlayerNode child, DateTime timestamp, int lifetimeMinutes = 5)
        {
            Parent = parent;
            Child = child;
            Timestamp = timestamp;
            Lifetime = lifetimeMinutes * 60 * 1000;

            ExpirationTimer = new Timer(OnExpirationReached, null, Lifetime, System.Threading.Timeout.Infinite);
        }

        public bool TryUpdateTimestamp(DateTime timestamp)
        {
            if (_isExpiring)
            {
                return false;
            }

            if (timestamp <= Timestamp)
            {
                return false;
            }

            Timestamp = timestamp;
            ExpirationTimer.Change(Lifetime, Timeout.Infinite);

            return true;
        }

        protected virtual void OnExpirationReached(object? state)
        {
            _isExpiring = true;

            var handler = ExpirationReached;
            handler?.Invoke(this, new EdgeExpirationEventArgs<PlayerEdge>(this));
        }
    }
}
