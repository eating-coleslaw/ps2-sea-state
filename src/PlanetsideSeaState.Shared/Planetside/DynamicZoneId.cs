namespace PlanetsideSeaState.Shared.Planetside
{
    public struct DynamicZoneId
    {
        /// <summary>
        /// The ID of the dynamic zone
        /// </summary>
        public int DefinitionId { get; } // zone ID

        /// <summary>
        /// The instance ID of the dynamic zone
        /// </summary>
        public int InstanceId { get; }

        /// <summary>
        /// The zone ID value as it would appear in census stream payloads' zone_id field
        /// </summary>
        public int ZoneId { get; }

        private const int _topMask = unchecked((int)0xFFFF0000);
        private const int _bottomMask = 0x0000FFFF;

        /// <summary>
        /// Create a new DynamicZoneId from a census stream payload's zone_id field
        /// </summary>
        /// <param name="zoneId">The zone_id value from a census strean payload</param>
        public DynamicZoneId(int zoneId)
        {
            ZoneId = zoneId;
            DefinitionId = ApplyMask(zoneId, _bottomMask);
            InstanceId = ApplyBitShiftRight(ApplyMask(zoneId, _topMask));
        }

        /// <summary>
        /// Create a new DynamicZoneId from known definition and instance IDs
        /// </summary>
        /// <param name="definitionId">The zone's definition ID</param>
        /// <param name="instanceId">The zone's instance ID, or 0 for non-instanced zones</param>
        public DynamicZoneId(int definitionId, int instanceId)
        {
            DefinitionId = definitionId;
            InstanceId = instanceId;

            ZoneId = ApplyBitShiftLeft(instanceId) | definitionId;
        }

        private static int ApplyMask(int input, int bitMask)
        {
            return input & bitMask;
        }

        private static int ApplyBitShiftRight(int input)
        {
            return input >> 16;
        }

        private static int ApplyBitShiftLeft(int input)
        {
            return input << 16;
        }
    }
}
