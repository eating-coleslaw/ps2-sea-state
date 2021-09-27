using static PlanetsideSeaState.App.CensusStream.WebsocketMonitorHelper;

namespace PlanetsideSeaState.App.CensusStream
{
    public interface IWebsocketMonitorHelper
    {
        WebsocketMonitorFactory GetFactory();
    }
}
