namespace PlanetsideSeaState.App.CensusStream.Models
{
    public class MetagameEventPayload : PayloadBase, IEquatablePayload<MetagameEventPayload>
    {
        public int InstanceId { get; set; }
        public int MetagameEventId { get; set; }
        public string MetagameEventState { get; set; }
        public float FactionVs { get; set; }
        public float FactionNc { get; set; }
        public float FactionTr { get; set; }
        public double ExperienceBonus { get; set; }


        #region IEquitable
        public override bool Equals(object obj)
        {
            return this.Equals(obj as MetagameEventPayload);
        }

        public bool Equals(MetagameEventPayload p)
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
                    && p.InstanceId == InstanceId
                    && p.WorldId == WorldId);
        }

        public static bool operator ==(MetagameEventPayload lhs, MetagameEventPayload rhs)
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

        public static bool operator !=(MetagameEventPayload lhs, MetagameEventPayload rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            var id = $"t{Timestamp:yyyyMMddTHHmmss}i{InstanceId}w{WorldId}";
            return id.GetHashCode();
        }
        #endregion IEquitable
    }
}
