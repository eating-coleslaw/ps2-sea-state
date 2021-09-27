﻿namespace PlanetsideSeaState.App.CensusStream.Models
{
    public class VehicleDestroyPayload : PayloadBase, IEquatablePayload<VehicleDestroyPayload>
    {
        public string AttackerCharacterId { get; set; }
        public int? AttackerLoadoutId { get; set; }
        public int? AttackerVehicleId { get; set; }
        public int? AttackerWeaponId { get; set; }
        public string CharacterId { get; set; }
        public int VehicleId { get; set; }
        public int? FactionId { get; set; }
        public int? FacilityId { get; set; }

        #region IEquitable
        public override bool Equals(object obj)
        {
            return this.Equals(obj as VehicleDestroyPayload);
        }

        public bool Equals(VehicleDestroyPayload p)
        {
            if (p is null)
            {
                return false;
            }

            if (ReferenceEquals(this, p))
            {
                return true;
            }

            if (this.GetType() != p.GetType())
            {
                return false;
            }

            return (p.Timestamp == Timestamp
                    && p.AttackerCharacterId == AttackerCharacterId
                    && p.VehicleId == VehicleId
                    && p.CharacterId == CharacterId);
        }

        public static bool operator ==(VehicleDestroyPayload lhs, VehicleDestroyPayload rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }

                return false;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(VehicleDestroyPayload lhs, VehicleDestroyPayload rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            var id = $"t{Timestamp:yyyyMMddTHHmmss}a{AttackerCharacterId}veh{VehicleId}v{CharacterId}";
            return id.GetHashCode();
        }
        #endregion IEquitable
    }
}
