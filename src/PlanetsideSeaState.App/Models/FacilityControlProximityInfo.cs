using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.Models
{
    public class FacilityControlProximityInfo
    {
        public int FacilityId { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid FacilityControlId { get; set; }
        public short NewFactionId { get; set; }
        public uint ZoneId { get; set; }

        public double TimeDiffSeconds { get; set; }
    }
}
