using System;

namespace PlanetsideSeaState.CensusServices.Models
{
    public class CensusOutfitMemberModel
    {
        public string CharacterId { get; set; }
        public string OutfitId { get; set; }
        public DateTime MemberSinceDate { get; set; }
        public string Rank { get; set; }
        public short RankOrdinal { get; set; }
    }
}
