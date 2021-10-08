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
        public bool IsCapture { get; set; }
        public short OldFactionId { get; set; }
        public short NewFactionId { get; set; }

        //public FacilityControl FacilityControl { get; set; }

        public int? AttributedPlayers { get; set; }

        public FactionTeamCounts TeamPlayers {get; set; }
        public FactionTeamCounts NsoTeamPlayers { get; set; }

        public int TotalPlayers { get; set; }
        public Dictionary<string, int> FactionPlayers { get; set; } = new()
        {
            { "VS", 0 },
            { "NC", 0 },
            { "TR", 0 },
            { "Unknown", 0 }
        };
        
        public int NsoPlayers { get; set; }
        public Dictionary<string, int> NsoFactionPlayers { get; set; } = new()
        {
            { "VS", 0 },
            { "NC", 0 },
            { "TR", 0 },
            { "Unknown", 0 }
        };

        //public int Vs { get; set; }
        //public int Nc { get; set; }
        //public int Tr { get; set; }
        //public int Uknown { get; set; }
        //public int Nso { get; set; }
        //public int Nso_Vs { get; set; }
        //public int Nso_Nc { get; set; }
        //public int Nso_Tr { get; set; }
        //public int Nso_Unknown { get; set; }

        public long? ElapsedMilliseconds { get; set; }
        public DateTime SearchBaseStartTime { get; set; }
        public DateTime SearchBaseEndTime { get; set; }
        public int SearchBasePlayerEvents { get; set;}
        public Dictionary<string, int> SearchBasePlayerEventTypes { get; set; } = new();

        public FacilityControlPopulations()
        {
        }

        public FacilityControlPopulations(FacilityControl facilityControl)
        {
            //FacilityControl = facilityControl;

            FacilityControlId = facilityControl.Id;
            FacilityId = facilityControl.FacilityId;
            Timestamp = facilityControl.Timestamp;
            WorldId = facilityControl.WorldId;
            ZoneId = facilityControl.ZoneId;
            //FacilityName = facilityControl.Name;
            IsCapture = facilityControl.IsCapture;
            OldFactionId = facilityControl.OldFactionId;
            NewFactionId = facilityControl.NewFactionId;
            AttributedPlayers = facilityControl.PlayerControls?.Count;
        }
    }
}
