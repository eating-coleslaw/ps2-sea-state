﻿using System;
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
        public int? FactionId { get; set; }
        public int? WorldId { get; set; }

        public int TeamOrdinal { get; set; }
        public long MatchId { get; set; }

        public Faction Faction { get; set; }
        public World World { get; set; }
        public Character LeaderCharacter { get; set; }
    }
}
