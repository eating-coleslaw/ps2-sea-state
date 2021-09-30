using System.Collections.Generic;

namespace PlanetsideSeaState.Shared.Constants
{
    public static class Zone
    {
        public const uint Indar = 2;
        public const uint Hossin = 4;
        public const uint Amerish = 6;
        public const uint Esamir = 8;

        public static List<uint> All => new()
        {
            Indar,
            Hossin,
            Amerish,
            Esamir
        };
    }
}
