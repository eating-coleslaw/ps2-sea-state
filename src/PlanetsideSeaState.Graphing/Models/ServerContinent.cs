namespace PlanetsideSeaState.Graphing.Models
{
    public struct ServerContinent
    {
        public int WorldId { get; }
        public int ZoneId { get; }

        public ServerContinent(int worldId, int zoneId)
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
