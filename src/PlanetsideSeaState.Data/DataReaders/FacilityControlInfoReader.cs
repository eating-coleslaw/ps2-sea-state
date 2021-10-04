using Npgsql;
using PlanetsideSeaState.Data.Models.QueryResults;
using System.Data;
using System.Text;

namespace PlanetsideSeaState.Data.DataReaders
{
    public class FacilityControlInfoReader : IDataReader<FacilityControlInfo>
    {
        public override FacilityControlInfo ReadEntry(NpgsqlDataReader reader)
        {
            FacilityControlInfo control = new();

            control.Id = reader.GetGuid("id");
            control.FacilityId = reader.GetInt32("facility_id");
            control.WorldId = reader.GetInt16("world_id");
            control.Timestamp = reader.GetDateTime("timestamp");
            control.FacilityName = reader.GetString("facility_name");
            control.IsCapture = reader.GetBoolean("is_capture");
            control.OldFactionId = reader.GetInt16("old_faction_id");
            control.NewFactionId = reader.GetInt16("new_faction_id");
            control.ZoneId = reader.GetUInt32("zone_id");
            control.MapRegionId = reader.GetInt32("map_region_id");
            control.FacilityTypeId = reader.GetInt32("facility_type_id");
            control.FacilityType = reader.GetString("facility_type");

            return control;
        }
    }
}
