using PlanetsideSeaState.App.CensusStream.Models;
using PlanetsideSeaState.Graphing.Models.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Graphing.Models.Relations
{
    public class PlayerRelation : PayloadBasedRelation
    {
        public string ActingPlayerId { get; private set; }
        public string RecipientPlayerId { get; private set; }

        public PlayerNode OtherPlayer { get; private set; }
        public int ZoneId { get; private set; }

        private readonly AutoResetEvent _autoEvent = new(true);

        public PlayerRelation(PlayerNode player, DateTime timestamp, PayloadEventType eventType, int zoneId, int? experienceId)
            : base(timestamp, eventType, experienceId)
        {
            OtherPlayer = player;
            ZoneId = zoneId;
        }

        public bool TryUpdate(DateTime timestamp, PayloadEventType eventType, int zoneId, int? experienceId)
        {
            _autoEvent.WaitOne();

            if (LastUpdate > timestamp)
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
    }
}
