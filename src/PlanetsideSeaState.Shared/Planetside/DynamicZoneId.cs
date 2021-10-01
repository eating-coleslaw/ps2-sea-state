namespace PlanetsideSeaState.Shared.Planetside
{
    public struct DynamicZoneId
    {
        /// <summary>
        /// The ID of the dynamic zone
        /// </summary>
        public ushort DefinitionId { get; } // zone ID

        /// <summary>
        /// The instance ID of the dynamic zone
        /// </summary>
        public ushort InstanceId { get; }

        /// <summary>
        /// The zone ID value as it would appear in census stream payloads' zone_id field
        /// </summary>
        public uint ZoneId { get; }

        private const uint _topMask = 0xFFFF0000;
        private const uint _bottomMask = 0x0000FFFF;

        /// <summary>
        /// Create a new DynamicZoneId from a census stream payload's zone_id field
        /// </summary>
        /// <param name="zoneId">The zone_id value from a census strean payload</param>
        public DynamicZoneId(uint zoneId)
        {
            ZoneId = zoneId;
            DefinitionId = (ushort)ApplyMask(zoneId, _bottomMask);
            InstanceId = (ushort)ApplyBitShiftRight(ApplyMask(zoneId, _topMask));
        }

        /// <summary>
        /// Create a new DynamicZoneId from known definition and instance IDs
        /// </summary>
        /// <param name="definitionId">The zone's definition ID</param>
        /// <param name="instanceId">The zone's instance ID, or 0 for non-instanced zones</param>
        public DynamicZoneId(ushort definitionId, ushort instanceId)
        {
            DefinitionId = definitionId;
            InstanceId = instanceId;

            ZoneId = ApplyBitShiftLeft(instanceId) | definitionId;
        }

        private static uint ApplyMask(uint input, uint bitMask)
        {
            return (input & bitMask);
        }

        private static uint ApplyBitShiftRight(uint input)
        {
            return input >> 16;
        }

        private static uint ApplyBitShiftLeft(uint input)
        {
            return input << 16;
        }
    }
}
