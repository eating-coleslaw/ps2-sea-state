using System.Collections.Generic;

namespace PlanetsideSeaState.CensusServices.Models
{
    public class CensusOutfitMemberCharactersModel : CensusOutfitModel
    {
        public IEnumerable<CensusOutfitMemberCharacterModel> Members { get; set; }
    }
}
