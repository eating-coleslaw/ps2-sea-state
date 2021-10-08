using Npgsql;
using PlanetsideSeaState.Data.Models.QueryResults;
using System.Data;
using System.Text;

namespace PlanetsideSeaState.Data.DataReaders
{
    class PlayerFacilityControlProximityReader : IDataReader<PlayerFacilityControlProximity>
    {
        public override PlayerFacilityControlProximity ReadEntry(NpgsqlDataReader reader)
        {
            PlayerFacilityControlProximity control = new();

            control.CharacterId = reader.GetString("acting_character_id");
            control.FacilityId = reader.GetInt32("recipient_character_id");
            control.Timestamp = reader.GetDateTime("timestamp");
            control.FacilityControlId = reader.GetGuid("facility_control_id");
            control.ZoneId = reader.GetUInt32("zone_id");
            control.TimeDiff = reader.GetTimeSpan(6);

            return control;
        }
    }
}
