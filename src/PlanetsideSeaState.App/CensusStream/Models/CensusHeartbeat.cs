﻿using System;

namespace PlanetsideSeaState.App.CensusStream.Models
{
    public class CensusHeartbeat
    {
        public DateTime LastHeartbeat { get; set; }
        public object Contents { get; set; }
    }
}
