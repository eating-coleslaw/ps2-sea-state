using System.ComponentModel.DataAnnotations;

namespace PlanetsideSeaState.Data.Models.Census
{
    public class Character
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }

        public bool IsOnline { get; set; }

        public string OutfitId { get; set; } = string.Empty;
        public string OutfitAlias { get; set; } = string.Empty;
        public string OutfitAliasLower { get; set; } = string.Empty;

        public short FactionId { get; set; }
        public short WorldId { get; set; }
        public short BattleRank { get; set; }
        public short PrestigeLevel { get; set; }

        #region Navigation Properties
        public OutfitMember OutfitMember { get; set; }
        #endregion Navigation Properties
    }
}
