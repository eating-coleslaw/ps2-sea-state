// Credit to Lampjaw

using DaybreakGames.Census;
using DaybreakGames.Census.JsonConverters;
using Newtonsoft.Json;

namespace PlanetsideSeaState.App.CensusStream
{
    public class StreamConstants
    {
        public static readonly JsonSerializer PayloadDeserializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            ContractResolver = new UnderscorePropertyNamesContractResolver(),
            Converters = new JsonConverter[]
                {
                    new BooleanJsonConverter(),
                    new DateTimeJsonConverter()
                }
        });
    }
}
