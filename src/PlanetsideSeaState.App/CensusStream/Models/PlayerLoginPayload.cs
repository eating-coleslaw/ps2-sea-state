namespace PlanetsideSeaState.App.CensusStream.Models
{
    public class PlayerLoginPayload : PayloadBase, IEquatablePayload<PlayerLoginPayload>
    {
        public string CharacterId { get; set; }

        #region IEquitable
        public override bool Equals(object obj)
        {
            return this.Equals(obj as PlayerLoginPayload);
        }

        public bool Equals(PlayerLoginPayload p)
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
                    && p.CharacterId == CharacterId);
        }

        public static bool operator ==(PlayerLoginPayload lhs, PlayerLoginPayload rhs)
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

        public static bool operator !=(PlayerLoginPayload lhs, PlayerLoginPayload rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            var id = $"t{Timestamp:yyyyMMddTHHmmss}c{CharacterId}";
            return id.GetHashCode();
        }
        #endregion IEquitable
    }
}
