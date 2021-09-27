using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.Models
{
    public class ActivityLeaderboardPlayerStats
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int? FactionId { get; set; }
        public int? BattleRank { get; set; }
        public int? PrestigeLevel { get; set; }
        public string OutfitId { get; set; }
        public string OutfitAlias { get; set; }

        //public bool? IsOnline { get; set; }
        //public DateTime? LoginDate { get; set; }
        //public DateTime? LogoutDate { get; set; }

        //// Session Length in seconds
        //public int? SessionLength { get; set; }
        //public int SessionKills { get; set; }

        public int ExpGains { get; set; }
        public int ExpAmount { get; set; }


        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Suicides { get; set; }
        public int Teamkills { get; set; }
        public int Headshots { get; set; }
        
        public int NsoKills { get; set; }
        public int NonNsoKills { get; set; }
        public int NsoTeamkills { get; set; }
        public int NonNsoTeamkills { get; set; }

        public int NsoDeaths { get; set; }
        public int NonNsoDeaths { get; set; }
        public int NsoTeamkillDeaths { get; set; }
        public int NonNsoTeamkillDeaths { get; set; }

        //public int DamageAssists { get; set; }

        //public int TopWeaponId { get; set; }
        //public string TopWeaponName { get; set; }

        //public int TopLoadoutId { get; set; }
    }
}
