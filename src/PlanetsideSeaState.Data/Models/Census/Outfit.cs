using System;
using System.ComponentModel.DataAnnotations;

namespace PlanetsideSeaState.Data.Models.Census
{
    public class Outfit
    {
        [Required]
        public string Id { get; set; }

        public string Name { get; set; }
        public string Alias { get; set; }
        public string AliasLower { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LeaderCharacterId { get; set; }
        public int MemberCount { get; set; }
        public int MembersOnlineCount { get; set; } = 0;
        public short? FactionId { get; set; }
        public short? WorldId { get; set; }

        public Character LeaderCharacter { get; set; }
    }
}
