using static PlanetsideSeaState.Graphing.Services.FacilityPopGraphServiceHelper;

namespace PlanetsideSeaState.Graphing.Services
{
    public interface IFacilityPopGraphServiceHelper
    {
        FacilityPopGraphServiceFactory GetFactory();
    }
}
