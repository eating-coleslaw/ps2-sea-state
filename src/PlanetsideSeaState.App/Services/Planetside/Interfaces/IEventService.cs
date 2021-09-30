using PlanetsideSeaState.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.Services.Planetside
{
    public interface IEventService
    {
        Task<ActivityLeaderboardStats> GetWorldPlayerStatsInTimeRange(short worldId, DateTime start, DateTime end, int take);
    }
}
