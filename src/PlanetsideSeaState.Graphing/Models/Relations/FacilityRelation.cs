//using PlanetsideSeaState.App.CensusStream.Models;
using PlanetsideSeaState.Graphing.Models.Nodes;
using PlanetsideSeaState.Shared.Planetside;
using System;
using System.Threading;

namespace PlanetsideSeaState.Graphing.Models.Relations
{
    public class FacilityRelation : PayloadBasedRelation
    {
        public PlayerNode Player { get; private set; }
        public FacilityNode Facility { get; private set; }
        public int FacilityId { get; }

        public int PlayerId { get; }

        public TimeSpan TimeSinceLastUpdate => DateTime.UtcNow - LastUpdate;

        private readonly AutoResetEvent _autoEvent = new(true);

        public FacilityRelation(PlayerNode playerNode, FacilityNode facilityNode, DateTime timestamp, PayloadEventType eventType, int? experienceId)
            : base(timestamp, eventType, experienceId)
        {
            Player = playerNode;
            Facility = facilityNode;
            FacilityId = facilityNode.Id;
        }

        public bool TryUpdate(DateTime timestamp, PayloadEventType eventType, int? experienceId)
        {
            _autoEvent.WaitOne();

            if (LastUpdate > timestamp)
            {
                return false;
            }

            LastUpdate = timestamp;
            EventType = eventType;
            ExperienceId = experienceId;

            _autoEvent.Set();

            return true;
        }
    }
}
