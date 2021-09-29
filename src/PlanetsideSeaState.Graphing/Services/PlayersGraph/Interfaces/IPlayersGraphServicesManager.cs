using PlanetsideSeaState.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Graphing.Services.PlayersGraph
{
    public interface IPlayersGraphServicesManager : IStatefulHostedService
    {
        PlayersGraphService GetService(int worldId);

        Task<bool> TryAddNewService(int worldId);
        Task<bool> TryRemoveService(int worldId);
    }
}
