using PlanetsideSeaState.App.CensusStream.Models;
using System;

namespace PlanetsideSeaState.App.CensusStream
{
    public interface IEquatablePayload<T> : IEquatable<T> where T : PayloadBase
    {
    }
}
