using System;

namespace PlanetsideSeaState.Graphing.Exceptions
{
    public class DuplicateGraphNodeException : Exception
    {
        public DuplicateGraphNodeException() : base()
        {
        }

        public DuplicateGraphNodeException(string message) : base(message)
        {
        }
    }
}
