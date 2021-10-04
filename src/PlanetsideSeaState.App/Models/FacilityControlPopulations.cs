using PlanetsideSeaState.Data.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetsideSeaState.App.Models
{
    public class FacilityControlPopulations
    {
        public Guid FacilityControlId { get; set; }
        public int FacilityId { get; set; }
        public DateTime Timestamp { get; set; }
        public short WorldId { get; set; }
        public uint ZoneId { get; set; }
        public string FacilityName { get; set; }

        public int Total { get; set; }
        public int Vs { get; set; }
        public int Nc { get; set; }
        public int Tr { get; set; }
        public int Nso { get; set; }

        public FacilityControlPopulations()
        {
        }

        public FacilityControlPopulations(FacilityControl facilityControl)
        {
            FacilityControlId = facilityControl.Id;
            FacilityId = facilityControl.FacilityId;
            Timestamp = facilityControl.Timestamp;
            WorldId = facilityControl.WorldId;
            ZoneId = facilityControl.ZoneId;
            //FacilityName = facilityControl.Name;
        }
    }
}
