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
    public class ProfileStore : IProfileStore
    {
        private readonly IProfileRepository _profileRepository;
        private readonly ILoadoutRepository _loadoutRepository;
        private readonly CensusProfile _censusProfile;
        private readonly CensusLoadout _censusLoadout;
        private readonly ILogger<ProfileStore> _logger;
        
        public string StoreName => "ProfileStore";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        public ProfileStore(IProfileRepository profileRepository, ILoadoutRepository loadoutRepository,
            CensusProfile censusProfile, CensusLoadout censusLoadout, ILogger<ProfileStore> logger)
        {
            _profileRepository = profileRepository;
            _loadoutRepository = loadoutRepository;
            _censusProfile = censusProfile;
            _censusLoadout = censusLoadout;
            _logger = logger;
        }

        public async Task<IEnumerable<Profile>> GetAllProfilesAsync()
        {
            return await _profileRepository.GetAllProfilesAsync();
        }

        public async Task<IEnumerable<Loadout>> GetAllLoadoutsAsync()
        {
            return await _loadoutRepository.GetAllLoadoutsAsync();
        }

        public Task<Profile> GetProfileFromLoadoutIdAsync(int loadoutId)
        {
            throw new NotImplementedException();
        }

        public async Task RefreshStore()
        {
            var profiles = await _censusProfile.GetAllProfilesAsync();

            var loadouts = await _censusLoadout.GetAllLoadoutsAsync();

            if (profiles != null)
            {
                await _profileRepository.UpsertRangeAsync(profiles.Select(ConvertToDbModel));
            }

            if (loadouts != null)
            {
                var allLoadouts = new List<CensusLoadoutModel>(loadouts);
                allLoadouts.AddRange(GetFakeNsCensusLoadoutModels());

                await _loadoutRepository.UpsertRangeAsync(allLoadouts.Select(ConvertToDbModel));
            }
        }

        private static Profile ConvertToDbModel(CensusProfileModel censusModel)
        {
            return new Profile
            {
                Id = censusModel.ProfileId,
                ProfileTypeId = censusModel.ProfileTypeId,
                FactionId = censusModel.FactionId,
                Name = censusModel.Name.English,
                ImageId = censusModel.ImageId
            };
        }

        private static Loadout ConvertToDbModel(CensusLoadoutModel censusModel)
        {
            return new Loadout
            {
                Id = censusModel.LoadoutId,
                ProfileId = censusModel.ProfileId,
                FactionId = censusModel.FactionId,
                CodeName = censusModel.CodeName,
            };
        }

        private static IEnumerable<CensusLoadoutModel> GetFakeNsCensusLoadoutModels()
        {
            var nsLoadouts = new List<CensusLoadoutModel>
            {
                GetNewCensusLoadoutModel(28, 190, 4, "NS Infiltrator"),
                GetNewCensusLoadoutModel(29, 191, 4, "NS Light Assault"),
                GetNewCensusLoadoutModel(30, 192, 4, "NS Combat Medic"),
                GetNewCensusLoadoutModel(31, 193, 4, "NS Engineer"),
                GetNewCensusLoadoutModel(32, 194, 4, "NS Heavy Assault"),
                GetNewCensusLoadoutModel(45, 252, 4, "NS Defector")
            };

            return nsLoadouts;
        }

        private static CensusLoadoutModel GetNewCensusLoadoutModel(int loadoutId, int profileId, int factionId, string codeName)
        {
            return new CensusLoadoutModel()
            {
                LoadoutId = loadoutId,
                ProfileId = profileId,
                FactionId = factionId,
                CodeName = codeName
            };
        }
    }
}
