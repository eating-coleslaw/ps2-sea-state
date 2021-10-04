//using PlanetsideSeaState.App.CensusStream.Models;
using PlanetsideSeaState.Shared.Planetside;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Graphing.Models.Relations
{
    public class PayloadBasedRelation
    {
        public DateTime LastUpdate { get; protected set; }
        public PayloadEventType EventType { get; protected set; }
        public int? ExperienceId { get; protected set; }

        //private readonly AutoResetEvent _autoEvent = new AutoResetEvent(true);

        public TimeSpan Age => DateTime.UtcNow - LastUpdate;

        public PayloadBasedRelation(DateTime timestamp, PayloadEventType eventType, int? experienceId)
        {
            LastUpdate = timestamp;
            EventType = eventType;
            ExperienceId = experienceId;
        }

        //public bool TryUpdateRelation(DateTime timestamp, PayloadEventType eventType, int? experienceId)
        //{
        //    _autoEvent.WaitOne();

        //    if (LastUpdate > timestamp)
        //    {
        //        return false;
        //    }

        //    LastUpdate = timestamp;
        //    EventType = eventType;
        //    ExperienceId = experienceId;

        //    _autoEvent.Set();

        //    return true;
        //}
    }
}
