﻿// Credit to Lampjaw @ Voidwell.DaybreakGames

using System;

namespace PlanetsideSeaState.App.CensusStream.Models
{
    public class StreamState
    {
        public DateTime LastStateChangeTime { get; set; }
        public object Contents { get; set; }
    }
}

