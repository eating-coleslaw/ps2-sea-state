namespace PlanetsideSeaState.Graphing.Models
{
    public struct ServerContinent
    {
        public short WorldId { get; }
        public uint ZoneId { get; }

        public ServerContinent(short worldId, uint zoneId)
        {
            WorldId = worldId;
            ZoneId = zoneId;
        }

        public override string ToString()
        {
            return $"{WorldId}^{ZoneId}";
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
