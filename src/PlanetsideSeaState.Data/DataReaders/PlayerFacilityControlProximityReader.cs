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

            control.CharacterId = reader.GetString("character_id");
            control.FacilityId = reader.GetInt32("facility_id");
            control.Timestamp = reader.GetDateTime("timestamp");
            control.FacilityControlId = reader.GetGuid("facility_control_id");
            control.NewFactionId = reader.GetInt16("new_faction_id");
            control.ZoneId = reader.GetUInt32("zone_id");
            control.TimeDiff = reader.GetTimeSpan(6);

            return control;
        }
    }
}
