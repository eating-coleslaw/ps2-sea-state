using static PlanetsideSeaState.CensusServices.Models.CensusCharacterModel;

namespace PlanetsideSeaState.CensusServices.Models
{
    public class CensusOutfitMemberCharacterModel : CensusOutfitMemberModel
    {
        public CharacterName Name { get; set; }

        public string OnlineStatus { get; set; }
        public short PrestigeLevel { get; set; }

        public string OutfitAlias { get; set; }
        public string OutfitAliasLower { get; set; }
    }
}
