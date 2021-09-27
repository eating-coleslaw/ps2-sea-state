namespace PlanetsideSeaState.CensusServices.Models
{
    public class CensusMetagameEventModel
    {
        public int MetagameEventId { get; set; }
        public MultiLanguageString Name { get; set; }
        public MultiLanguageString Description { get; set; }
        public int Type { get; set; }
        public int ExperienceBonus { get; set; }
    }
}
