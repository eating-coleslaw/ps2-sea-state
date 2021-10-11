using PlanetsideSeaState.Data.Models.Events;
using PlanetsideSeaState.Data.Models.QueryResults;
using PlanetsideSeaState.Shared.Constants;
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

        public long? ElapsedMilliseconds { get; set; }
        public DateTime SearchBaseStartTime { get; set; }
        public DateTime SearchBaseEndTime { get; set; }
        public int SearchBasePlayerEvents { get; set;}
        public Dictionary<string, int> SearchBasePlayerEventTypes { get; set; } = new();
        

        public Dictionary<string, HashSet<FacilityControlPopulationPlayer>> TeamPlayerDetails { get; private set; } = new()
        {
            { "VS", new() },
            { "NC", new() },
            { "TR", new() },
            { "Unknown", new() }
        };

        public IDictionary<Guid, int> CloseFacilityControlPlayerCounts { get; set; }

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

        public void AddPlayerDetails(FacilityControlPopulationPlayer player)
        {
            switch (player.TeamId)
            {
                case Faction.VS:
                    TeamPlayerDetails["VS"].Add(player);
                    break;

                case Faction.NC:
                    TeamPlayerDetails["NC"].Add(player);
                    break;

                case Faction.TR:
                    TeamPlayerDetails["TR"].Add(player);
                    break;

                default:
                    TeamPlayerDetails["Unknown"].Add(player);
                    break;
            };
        }
    }
}
