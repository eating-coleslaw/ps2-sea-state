using PlanetsideSeaState.CensusStore.Services;
using PlanetsideSeaState.Data.Models.Census;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.Services.Planetside
{
    public class WorldService : IWorldService
    {
        private readonly IWorldStore _worldStore;
        private readonly ILogger<ProfileService> _logger;


        public WorldService(IWorldStore worldStore, ILogger<ProfileService> logger)
        {
            _worldStore = worldStore;
            _logger = logger;
        }


        public async Task<IEnumerable<World>> GetAllWorlds()
        {
            return await _worldStore.GetAllWorldsAsync();
        }

        public async Task<World> GetWorld(int worldId)
        {
            return await _worldStore.GetWorldByIdAsync(worldId);
        }

        public static bool IsJaegerWorldId(int worldId)
        {
            return worldId == 19;
        }
    }
}
