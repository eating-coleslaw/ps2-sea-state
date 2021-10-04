using Npgsql;
using PlanetsideSeaState.Data.Models.QueryResults;
using PlanetsideSeaState.Shared.Planetside;
using System.Data;
using System.Text;

namespace PlanetsideSeaState.Data.DataReaders
{
    public class PlayerConnectionEventReader : IDataReader<PlayerConnectionEvent>
    {
        public override PlayerConnectionEvent ReadEntry(NpgsqlDataReader reader)
        {
            PlayerConnectionEvent control = new();

            control.ActingCharacterId= reader.GetString("acting_character_id");
            control.RecipientCharacterId= reader.GetString("recipient_character_id");
            control.Timestamp = reader.GetDateTime("timestamp");
            control.EventType = (PayloadEventType)reader.GetInt32("event_type");
            control.ActingFactionId = reader.GetInt16("acting_faction_id");
            control.RecipientFactionId = reader.GetInt16("recipient_faction_id");
            control.ExperienceId = reader.GetNullableInt32("experience_id");
            control.ZoneId = reader.GetUInt32("zone_id");

            return control;
        }
    }
}
