namespace PlanetsideSeaState.App.CensusStream.Models
{
    public class PlayerFacilityCapturePayload : PayloadBase, IEquatablePayload<PlayerFacilityCapturePayload>
    {
        public string CharacterId { get; set; }
        public int FacilityId { get; set; }
        public string OutfitId { get; set; }

        #region IEquitable
        public override bool Equals(object obj)
        {
            return this.Equals(obj as PlayerFacilityCapturePayload);
        }

        public bool Equals(PlayerFacilityCapturePayload p)
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
                    && p.CharacterId == CharacterId
                    && p.FacilityId == FacilityId);
        }

        public static bool operator ==(PlayerFacilityCapturePayload lhs, PlayerFacilityCapturePayload rhs)
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

        public static bool operator !=(PlayerFacilityCapturePayload lhs, PlayerFacilityCapturePayload rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            var id = $"t{Timestamp:yyyyMMddTHHmmss}f{FacilityId}c{CharacterId}";
            return id.GetHashCode();
        }
        #endregion IEquitable
    }
}
