using PlanetsideSeaState.Data.Models.Census;
using System;
using System.ComponentModel.DataAnnotations;

namespace PlanetsideSeaState.Data.Models.Events
{
    public class ExperienceGain
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public int ExperienceId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        public short WorldId { get; set; }
        public uint ZoneId { get; set; }
        public int Amount { get; set; }
        public short? LoadoutId { get; set; }
        public string OtherId { get; set; }

        #region Navigation Properties
        public Character ActingCharacter { get; set; }
        public Character RecipientCharacter { get; set; }
        #endregion Navigation Properties
    }
}
