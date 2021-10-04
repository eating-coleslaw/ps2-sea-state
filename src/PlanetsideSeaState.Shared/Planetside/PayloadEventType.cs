namespace PlanetsideSeaState.Shared.Planetside
{
    public enum PayloadEventType
    {
        ContinentLock,
        ContinentUnlock,
        Death = 2,
        FacilityControl,
        GainExperience = 4,
        MetagameEvent,
        PlayerFacilityCapture,
        PlayerFacilityDefend,
        PlayerLogin,
        PlayerLogout,
        VehicleDestroy = 10
    }
}
