namespace PlanetsideSeaState.Shared.Constants
{
    public static class Faction
    {
        public const short Unknown = -1;
        public const short VS = 1;
        public const short NC = 2;
        public const short TR = 3;
        public const short NSO = 4;

        public static string GetAbbreviation(short? factionId)
        {
            return factionId switch
            {
                VS => "VS",
                NC => "NC",
                TR => "TR",
                NSO => "NSO",
                _ => "?"
            };
        }
    }
}
