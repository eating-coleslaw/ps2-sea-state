using PlanetsideSeaState.Shared.Planetside;
using PlanetsideSeaState.Data.Models.Census;
using System;
using System.ComponentModel.DataAnnotations;

namespace PlanetsideSeaState.Data.Models.Events
{
    public class VehicleDestruction
    {
        [Required]
        public DateTime Timestamp { get; set; }
        [Required]
        public string AttackerCharacterId { get; set; }
        [Required]
        public string VictimCharacterId { get; set; }
        [Required]
        public int VictimVehicleId { get; set; }

        public DeathType DeathType { get; set; }

        public int? AttackerVehicleId { get; set; }
        public int? WeaponId { get; set; }
        public bool? IsVehicleWeapon { get; set; }
        public short WorldId { get; set; }
        public uint? ZoneId { get; set; }
        public int? FacilityId { get; set; }


        #region Navigation Properties
        public Character AttackerCharacter { get; set; }
        public Character VictimCharacter { get; set; }
        public MapRegion Facility { get; set; }
        #endregion Navigation Properties
    }
}