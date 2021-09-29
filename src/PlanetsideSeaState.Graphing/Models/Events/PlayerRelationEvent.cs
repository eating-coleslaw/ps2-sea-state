using PlanetsideSeaState.App.CensusStream.Models;
using PlanetsideSeaState.Data.Models.Census;
using PlanetsideSeaState.Data.Models.Events;
using System;

namespace PlanetsideSeaState.Graphing.Models.Events
{
    public class PlayerRelationEvent
    {
        public Character ActingCharacter { get; }
        public Character RecipientCharacter { get; }
        public DateTime Timestamp { get; }
        public PayloadEventType EventType { get; }
        public int? ZoneId { get; }
        public int WorldId { get; }
        public int? ExperienceId { get; }

        public PlayerRelationEvent(Death death)
        {
            ActingCharacter = death.AttackerCharacter;
            RecipientCharacter = death.VictimCharacter;
            Timestamp = death.Timestamp;
            EventType = PayloadEventType.Death;
            ZoneId = death.ZoneId;
            WorldId = death.WorldId;
        }

        public PlayerRelationEvent(ExperienceGain experienceGain)
        {
            ActingCharacter = experienceGain.ActingCharacter;
            RecipientCharacter = experienceGain.RecipientCharacter;
            Timestamp = experienceGain.Timestamp;
            EventType = PayloadEventType.GainExperience;
            ZoneId = experienceGain.ZoneId;
            WorldId = experienceGain.WorldId;
            ExperienceId = experienceGain.ExperienceId;
        }

        public PlayerRelationEvent(VehicleDestruction destruction)
        {
            ActingCharacter = destruction.AttackerCharacter;
            RecipientCharacter = destruction.VictimCharacter;
            Timestamp = destruction.Timestamp;
            EventType = PayloadEventType.VehicleDestroy;
            ZoneId = destruction.ZoneId;
            WorldId = destruction.WorldId;
        }
    }
}
