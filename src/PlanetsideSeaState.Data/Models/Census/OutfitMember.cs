using System;
using System.ComponentModel.DataAnnotations;

namespace PlanetsideSeaState.Data.Models.Census
{
    public class OutfitMember
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public string OutfitId { get; set; }

        public short FactionId { get; set; }

        public DateTime? MemberSinceDate { get; set; }
        public string Rank { get; set; }
        public short? RankOrdinal { get; set; }

        public Character Character { get; set; }
        public Outfit Outfit { get; set; }
    }
}
