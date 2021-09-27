using PlanetsideSeaState.CensusStore.Services;
using PlanetsideSeaState.Data.Models.Census;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.Services.Planetside
{
    public class FactionService : IFactionService
    {
        private readonly IFactionStore _factionStore;
        private readonly ILogger<FactionService> _logger;


        public FactionService(IFactionStore factionStore, ILogger<FactionService> logger)
        {
            _factionStore = factionStore;
            _logger = logger;
        }

        public async Task<IEnumerable<Faction>> GetAllFactions()
        {
            return await _factionStore.GetAllFactionsAsync();
        }

        public async Task<Faction> GetFaction(int factionId)
        {
            return await _factionStore.GetFactionByIdAsync(factionId);
        }

        public static string GetFactionAbbrevFromId(int factionId)
        {
            return factionId switch
            {
                1 => "VS",
                2 => "NC",
                3 => "TR",
                4 => "NSO",
                _ => string.Empty
            };
        }
    }
}
