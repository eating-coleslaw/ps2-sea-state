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
        public int WorldId { get; set; }
        public int? ZoneId { get; set; }
        public int? FacilityId { get; set; }


        #region Navigation Properties
        public Character AttackerCharacter { get; set; }
        public Character VictimCharacter { get; set; }
        public Vehicle AttackerVehicle { get; set; }
        public Vehicle VictimVehicle { get; set; }
        public Item Weapon { get; set; }
        public World World { get; set; }
        public Zone Zone { get; set; }
        public MapRegion Facility { get; set; }
        #endregion Navigation Properties
    }
}