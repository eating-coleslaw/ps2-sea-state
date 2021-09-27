// Credit to Lampjaw

using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.CensusStream
{
    public interface IEventProcessorHandler
    {
        Task<bool> TryProcessAsync(string eventName, JToken payload);
    }
}
