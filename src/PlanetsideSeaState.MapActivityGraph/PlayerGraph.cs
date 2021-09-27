using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetsideSeaState.MapActivityGraph
{
    public class PlayerGraph
    {
        private List<PlayerNode> PlayersNodes { get; set; }
        private List<PlayerEdge> PlayerEdges { get; set; }
    }
}
