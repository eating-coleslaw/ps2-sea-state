﻿using PlanetsideSeaState.App.CensusStream.Models;
using PlanetsideSeaState.Graphing.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Graphing
{
    public class PlayerEdge
    {
        public readonly PlayerNode Parent;
        public readonly PlayerNode Child;

        public DateTime LastUpdate { get; set; }
        public int? ZoneId { get; private set; }
        public PayloadEventType EventType { get; private set; }
        public int? ExperienceId { get; private set; }

        // Max time, in milliseconds, before the edge expires
        public int Lifetime { get; private set; }
        private Timer ExpirationTimer { get; set; }
        public event EventHandler<EdgeExpirationEventArgs<PlayerEdge>> ExpirationReached;

        private bool _isExpiring = false;

        private readonly AutoResetEvent _autoEvent = new(true);

        public PlayerEdge(PlayerNode parent, PlayerNode child, PlayerRelationEvent relationEvent, int lifetimeMinutes = 5)
        {
            Parent = parent;
            Child = child;
            LastUpdate = relationEvent.Timestamp;
            EventType = relationEvent.EventType;
            ExperienceId = relationEvent.ExperienceId;
            ZoneId = relationEvent.ZoneId;
            Lifetime = lifetimeMinutes * 60 * 1000;

            ExpirationTimer = new Timer(OnExpirationReached, null, Lifetime, System.Threading.Timeout.Infinite);
        }

        public PlayerEdge(PlayerNode parent, PlayerNode child, DateTime timestamp, PayloadEventType eventType, int zoneId, int? experienceId, int lifetimeMinutes = 5)
        {
            Parent = parent;
            Child = child;
            LastUpdate = timestamp;
            EventType = eventType;
            ExperienceId = experienceId;
            ZoneId = zoneId;
            Lifetime = lifetimeMinutes * 60 * 1000;

            ExpirationTimer = new Timer(OnExpirationReached, null, Lifetime, System.Threading.Timeout.Infinite);
        }

        public bool TryUpdate(PlayerRelationEvent relationEvent)
        {
            if (_isExpiring)
            {
                return false;
            }

            _autoEvent.WaitOne();

            if (LastUpdate >= relationEvent.Timestamp)
            {
                _autoEvent.Set();
                return false;
            }

            LastUpdate = relationEvent.Timestamp;
            EventType = relationEvent.EventType;
            ZoneId = relationEvent.ZoneId;
            ExperienceId = relationEvent.ExperienceId;

            _autoEvent.Set();

            return true;
        }

        public bool TryUpdate(DateTime timestamp, PayloadEventType eventType, int zoneId, int? experienceId)
        {
            if (_isExpiring)
            {
                return false;
            }
            
            _autoEvent.WaitOne();

            if (LastUpdate >= timestamp)
            {
                _autoEvent.Set();
                return false;
            }

            LastUpdate = timestamp;
            EventType = eventType;
            ZoneId = zoneId;
            ExperienceId = experienceId;

            _autoEvent.Set();

            return true;
        }

        // TODO: delete this
        public bool TryUpdateTimestamp(DateTime timestamp)
        {
            if (_isExpiring)
            {
                return false;
            }

            if (timestamp <= LastUpdate)
            {
                return false;
            }

            LastUpdate = timestamp;
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
