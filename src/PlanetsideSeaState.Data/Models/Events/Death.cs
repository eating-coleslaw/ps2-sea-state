using PlanetsideSeaState.Shared.Planetside;
using PlanetsideSeaState.Data.Models.Census;
using System;
using System.ComponentModel.DataAnnotations;

namespace PlanetsideSeaState.Data.Models.Events
{
    public class Death
    {
        [Required]
        public DateTime Timestamp { get; set; }
        
        [Required]
        public string AttackerCharacterId { get; set; }

        [Required]
        public string VictimCharacterId { get; set; }

        public DeathType DeathType { get; set; }

        public short? AttackerLoadoutId { get; set; }
        public short? AttackerFactionId { get; set; }
        
        public short? VictimLoadoutId { get; set; }
        public short? VictimFactionId { get; set; }

        public bool IsHeadshot { get; set; }

        public int? WeaponId { get; set; }
        public int? AttackerVehicleId { get; set; }

        public uint? ZoneId { get; set; }
        public short WorldId { get; set; }


        #region Navigation Properties
        public Character AttackerCharacter { get; set; }
        public Character VictimCharacter { get; set; }
        #endregion Navigation Properties
    }
}
