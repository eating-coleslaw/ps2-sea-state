using PlanetsideSeaState.Data.Models.Census;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PlanetsideSeaState.Data.Models.Events
{
    public class PlayerFacilityControl
    {
        [Required]
        public int FacilityId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }
        [Required]
        public string CharacterId { get; set; }

        public Guid FacilityControlId { get; set; }

        public bool IsCapture { get; set; }

        public short WorldId { get; set; }
        public uint ZoneId { get; set; }
        public string OutfitId { get; set; }

        #region Navigation Properties
        public Character Character { get; set; }
        public MapRegion Facility { get; set; }
        
        [JsonIgnore]
        public FacilityControl FacilityControl { get; set; }
        #endregion Navigation Properties
    }
}
