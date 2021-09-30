namespace PlanetsideSeaState.CensusServices.Models
{
    public class CensusFacilityLinkModel
    {
        public uint ZoneId { get; set; }
        public int FacilityIdA { get; set; }
        public int FacilityIdB { get; set; }
        public string Description { get; set; }
    }
}
