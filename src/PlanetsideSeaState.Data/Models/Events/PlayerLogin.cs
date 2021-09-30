using PlanetsideSeaState.Data.Models.Census;
using System;
using System.ComponentModel.DataAnnotations;

namespace PlanetsideSeaState.Data.Models.Events
{
    public class PlayerLogin
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        public short WorldId { get; set; }

        #region Navigation Properties
        public Character Character { get; set; }
        #endregion Navigation Properties
    }
}
