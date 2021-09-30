using System.Collections.Generic;

namespace PlanetsideSeaState.Shared.Constants
{
    public static class World
    {
        public const short Connery = 1;
        public const short Miller = 10;
        public const short Cobalt = 13;
        public const short Emerald = 17;
        public const short Jaeger = 19;
        public const short SolTech = 40;

        public static List<short> All => new()
        {
            Connery,
            Miller,
            Cobalt,
            Emerald,
            Jaeger,
            SolTech
        };

        /// <summary>
        /// Checks whether a given ID corresponds to an existing world
        /// </summary>
        /// <param name="worldId">The world ID to verify</param>
        /// <returns></returns>
        public static bool IsValid(short worldId)
        {
            return worldId == Connery
                    || worldId == Miller
                    || worldId == Cobalt
                    || worldId == Emerald
                    || worldId == Jaeger
                    || worldId == SolTech;
        }

        /// <summary>
        /// Get the display name for a world
        /// </summary>
        /// <param name="worldId">ID of the world</param>
        /// <returns></returns>
        public static string GetName(short worldId)
        {
            return worldId switch
            {
                Connery => "Connery",
                Miller => "Miller",
                Cobalt => "Cobalt",
                Emerald => "Emerald",
                Jaeger => "Jaeger",
                SolTech => "SolTech",
                _ => $"Unknown World ({worldId})",
            };
        }
    }
}
