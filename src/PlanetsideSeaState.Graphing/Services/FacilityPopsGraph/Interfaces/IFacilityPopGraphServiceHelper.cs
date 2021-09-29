using static PlanetsideSeaState.Graphing.Services.FacilityPopsGraph.FacilityPopGraphServiceHelper;

namespace PlanetsideSeaState.Graphing.Services.FacilityPopsGraph
{
    public interface IFacilityPopGraphServiceHelper
    {
        FacilityPopGraphServiceFactory GetFactory();
    }
}
