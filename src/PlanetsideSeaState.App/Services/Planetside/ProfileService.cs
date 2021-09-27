using PlanetsideSeaState.CensusStore.Services;
using PlanetsideSeaState.Data.Models.Census;
using Microsoft.Extensions.Logging;
using PlanetsideSeaState.Shared.Planetside;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.Services.Planetside
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileStore _profileStore;
        private readonly ILogger<ProfileService> _logger;
        

        public ProfileService(IProfileStore profileStore, ILogger<ProfileService> logger)
        {
            _profileStore = profileStore;
            _logger = logger;
        }

        public async Task<IEnumerable<Profile>> GetAllProfiles()
        {
            return await _profileStore.GetAllProfilesAsync();
        }

        public async Task<Profile> GetProfileFromLoadoutId(int loadoutId)
        {
            return await _profileStore.GetProfileFromLoadoutIdAsync(loadoutId);
        }

        public PlanetsideClass GetPlanetsideClassFromLoadoutId(int loadoutId)
        {
            return loadoutId switch
            {
                // NC                
                1 => PlanetsideClass.Infiltrator,
                3 => PlanetsideClass.LightAssault,
                4 => PlanetsideClass.Medic,
                5 => PlanetsideClass.Engineer,
                6 => PlanetsideClass.HeavyAssault,
                7 => PlanetsideClass.MAX,

                // TR
                8 => PlanetsideClass.Infiltrator,
                10 => PlanetsideClass.LightAssault,
                11 => PlanetsideClass.Medic,
                12 => PlanetsideClass.Engineer,
                13 => PlanetsideClass.HeavyAssault,
                14 => PlanetsideClass.MAX,

                // VS
                15 => PlanetsideClass.Infiltrator,
                17 => PlanetsideClass.LightAssault,
                18 => PlanetsideClass.Medic,
                19 => PlanetsideClass.Engineer,
                20 => PlanetsideClass.HeavyAssault,
                21 => PlanetsideClass.MAX,

                // NS
                28 => PlanetsideClass.Infiltrator,
                29 => PlanetsideClass.LightAssault,
                30 => PlanetsideClass.Medic,
                31 => PlanetsideClass.Engineer,
                32 => PlanetsideClass.HeavyAssault,
                45 => PlanetsideClass.MAX,

                _ => throw new ArgumentException($"{loadoutId} is not a valid loadout ID")
            };
        }

        public string GetPlanetsideClassDisplayName(PlanetsideClass planetsideClass)
        {
            var enumName = Enum.GetName(typeof(PlanetsideClass), planetsideClass);

            return Regex.Replace(enumName, @"(\p{Ll})(\p{Lu})", "$1 $2");
        }

        public static bool IsMaxLoadoutId(int? loadoutId)
        {
            return loadoutId switch
            {
                7 => true,
                14 => true,
                21 => true,
                45 => true,
                null => false,
                _ => false,
            };
        }

        public static bool IsMaxProfileId(int profileId)
        {
            return profileId switch
            {
                8 => true,
                16 => true,
                23 => true,
                252 => true,
                _ => false,
            };
        }
    }
}
