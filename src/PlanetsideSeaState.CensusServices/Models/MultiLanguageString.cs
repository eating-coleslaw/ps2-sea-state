// Credit to Lampjaw

using Newtonsoft.Json;

namespace PlanetsideSeaState.CensusServices.Models
{
    public class MultiLanguageString
    {
        [JsonProperty("en")]
        public string English { get; set; }
    }
}
