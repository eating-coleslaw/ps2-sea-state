namespace PlanetsideSeaState.Shared.Constants
{
    public static class Loadout
    {
        public const short VS_Infiltrator = 15;
        public const short VS_LightAssault = 17;
        public const short VS_Medic = 18;
        public const short VS_Engineer = 19;
        public const short VS_HeavyAssault = 20;
        public const short VS_MAX = 21;

        public const short NC_Infiltrator = 1;
        public const short NC_LightAssault = 3;
        public const short NC_Medic = 4;
        public const short NC_Engineer = 5;
        public const short NC_HeavyAssault = 6;
        public const short NC_MAX = 7;

        public const short TR_Infiltrator = 8;
        public const short TR_LightAssault = 10;
        public const short TR_Medic = 11;
        public const short TR_Engineer = 12;
        public const short TR_HeavyAssault = 13;
        public const short TR_MAX = 14;

        public const short NS_Infiltrator = 28;
        public const short NS_LightAssault = 29;
        public const short NS_Medic = 30;
        public const short NS_Engineer = 31;
        public const short NS_HeavyAssault = 32;
        public const short NS_MAX = 45;

        /// <summary>
        /// Get the faction ID to which a loadout ID belongs
        /// </summary>
        /// <param name="loadoutID">ID of the loadout</param>
        /// <returns>Faction ID of the loadout, or -1 if the loadout ID is not valid</returns>
        public static short GetFaction(short loadoutID)
        {
            if (loadoutID == NC_Infiltrator
                    || loadoutID == NC_LightAssault
                    || loadoutID == NC_Medic
                    || loadoutID == NC_Engineer
                    || loadoutID == NC_HeavyAssault
                    || loadoutID == NC_MAX)
            {
                return Faction.NC;
            }
            else if (loadoutID == VS_Infiltrator
                  || loadoutID == VS_LightAssault
                  || loadoutID == VS_Medic
                  || loadoutID == VS_Engineer
                  || loadoutID == VS_HeavyAssault
                  || loadoutID == VS_MAX)
            {
                return Faction.VS;
            }
            else if (loadoutID == TR_Infiltrator
                  || loadoutID == TR_LightAssault
                  || loadoutID == TR_Medic
                  || loadoutID == TR_Engineer
                  || loadoutID == TR_HeavyAssault
                  || loadoutID == TR_MAX)
            {
                return Faction.TR;
            }
            else if (loadoutID == NS_Infiltrator
                  || loadoutID == NS_LightAssault
                  || loadoutID == NS_Medic
                  || loadoutID == NS_Engineer
                  || loadoutID == NS_HeavyAssault
                  || loadoutID == NS_MAX)
            {
                return Faction.NSO;
            }

            return Faction.Unknown;
        }

    }
}
