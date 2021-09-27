using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.CensusStream
{
    public interface IWebsocketEventHandler
    {
        Task Process(JToken jPayload);
    }
}
