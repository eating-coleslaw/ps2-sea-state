namespace PlanetsideSeaState.App.CensusStream.Models
{
    /*
     * Damage Assists: sent when a player who died had recieved damage from another player in addition to the player who killed them. (TODO: teamkills? self?)
     * Grenade Assists: when a player kills another player, a message is sent for each status grenade effect with which the dead player was afflicted
     * Spot Assist: sent when a player dies while spotted
     * Revive: sent when a player accepts a revive that generates experience (e.g. player who died to a teamkill are not counted)
     */
    public class GainExperiencePayload : PayloadBase, IEquatablePayload<GainExperiencePayload>
    {
        /*
         * Damage Assists: the player who damaged but didn't kill the dead player
         * Grenade Assists: the player who killed the status grenade-afflicted player
         * Spot Assist: the player who spotted the dead player (TODO: first to spot? last? all?)
         * Revive: the player who did the reviving
        */
        public string CharacterId { get; set; }
        public int ExperienceId { get; set; }
        public int Amount { get; set; }
        public int? LoadoutId { get; set; }

        /*
         * Damage Assists: the player who died
         * Grenade Assists: the player who died
         * Spot Assist: the player who died
         * Revive: the player who was revived
        */
        public string OtherId { get; set; }

        #region IEquitable
        public override bool Equals(object obj)
        {
            return this.Equals(obj as GainExperiencePayload);
        }

        public bool Equals(GainExperiencePayload p)
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
                    && p.ExperienceId == ExperienceId
                    && p.OtherId == OtherId);
        }

        public static bool operator ==(GainExperiencePayload lhs, GainExperiencePayload rhs)
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

        public static bool operator !=(GainExperiencePayload lhs, GainExperiencePayload rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            var id = $"t{Timestamp:yyyyMMddTHHmmss}a{CharacterId}e{ExperienceId}r{OtherId}";
            return id.GetHashCode();
        }
        #endregion IEquitable
    }
}
