using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.Models
{
    public class ActivityLeaderboardStats
    {
        public IEnumerable<ActivityLeaderboardPlayerStats> Players { get; set; }
    }
}
