using PlanetsideSeaState.CensusServices;
using PlanetsideSeaState.CensusServices.Models;
using PlanetsideSeaState.Data.Models.Census;
using PlanetsideSeaState.Data.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanetsideSeaState.CensusStore.Services
{
    public class WorldStore : IWorldStore
    {
        private readonly IWorldRepository _worldRepository;
        private readonly CensusWorld _censusWorld;
        private readonly ILogger<WorldStore> _logger;

        public string StoreName => "WorldStore";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public WorldStore(IWorldRepository worldRepository, CensusWorld censusWorld, ILogger<WorldStore> logger)
        {
            _worldRepository = worldRepository;
            _censusWorld = censusWorld;
            _logger = logger;
        }

        public async Task<IEnumerable<World>> GetAllWorldsAsync()
        {
            return await _worldRepository.GetAllWorldsAsync();
        }

        public async Task<World> GetWorldByIdAsync(int worldId)
        {
            return await _worldRepository.GetWorldByIdAsync(worldId);
        }

        public async Task RefreshStore()
        {
            var worlds = await _censusWorld.GetAllWorlds();

            if (worlds != null)
            {
                await _worldRepository.UpsertRangeAsync(worlds.Select(ConvertToDbModel));
            }
        }

        private static World ConvertToDbModel(CensusWorldModel censusModel)
        {
            return new World
            {
                Id = censusModel.WorldId,
                Name = censusModel.Name.English
            };
        }

        public static bool IsJaegerWorldId(int worldId)
        {
            return worldId == 19;
        }
    }
}
