using static PlanetsideSeaState.Graphing.Services.PlayersGraph.PlayersGraphServiceHelper;

namespace PlanetsideSeaState.Graphing.Services.PlayersGraph
{
    public interface IPlayersGraphServiceHelper
    {
        PlayersGraphServiceFactory GetFactory();
    }
}
