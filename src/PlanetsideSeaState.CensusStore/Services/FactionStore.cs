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
    public class FactionStore : IFactionStore
    {
        private readonly IFactionRepository _factionRepository;
        private readonly CensusFaction _censusFaction;
        private readonly ILogger<FactionStore> _logger;

        public string StoreName => "FactionStore";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public FactionStore(IFactionRepository factionRepository, CensusFaction censusFaction, ILogger<FactionStore> logger)
        {
            _factionRepository = factionRepository;
            _censusFaction = censusFaction;
            _logger = logger;
        }

        public async Task<IEnumerable<Faction>> GetAllFactionsAsync()
        {
            return await _factionRepository.GetAllFactionsAsync();
        }

        public async Task<Faction> GetFactionByIdAsync(int factionId)
        {
            return await _factionRepository.GetFactionByIdAsync(factionId);
        }

        public async Task RefreshStore()
        {
            var factions = await _censusFaction.GetAllFactions();


            if (factions != null)
            {
                await _factionRepository.UpsertRangeAsync(factions.Select(ConvertToDbModel));
            }
        }

        private static Faction ConvertToDbModel(CensusFactionModel censusModel)
        {
            return new Faction
            {
                Id = censusModel.FactionId,
                Name = censusModel.Name.English,
                ImageId = censusModel.ImageId,
                CodeTag = censusModel.CodeTag,
                UserSelectable = censusModel.UserSelectable
            };
        }
    }
}
