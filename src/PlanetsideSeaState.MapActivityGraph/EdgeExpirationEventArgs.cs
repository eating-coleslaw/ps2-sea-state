using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetsideSeaState.MapActivityGraph
{
    public class EdgeExpirationEventArgs<T> : EventArgs
    {
        public T Edge { get; private set; }

        public EdgeExpirationEventArgs(T edge)
        {
            Edge = edge;
        }
    }
}
