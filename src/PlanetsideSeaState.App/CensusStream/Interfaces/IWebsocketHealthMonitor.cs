// Credit to Lampjaw

using System;

namespace PlanetsideSeaState.App.CensusStream
{
    public interface IWebsocketHealthMonitor
    {
        bool IsHealthy();
        void ReceivedEvent(int worldId, string eventName, DateTime? timestamp = null);
        void ClearWorld(int worldId);
        void ClearAllWorlds();
    }
}