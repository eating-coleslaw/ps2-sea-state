CREATE OR REPLACE FUNCTION GetRecentFacilityControls(
  i_worldId smallint DEFAULT NULL,
  i_facilityId int DEFAULT NULL,
  i_rowLimit smallint DEFAULT 20
)
  RETURNS TABLE (
    Id uuid,
    Facility_Id int,
    World_Id smallint,
    "timestamp" timestamp without time zone,
    Facility_Name text,
    Is_Capture boolean,
    Old_Faction_Id smallint,
    New_Faction_Id smallint,
    Zone_Id bigint,
    Map_Region_Id int,
    Facility_Type_Id int,
    Facility_Type text
  )
  LANGUAGE plpgsql
AS
$BODY$
BEGIN

  RETURN QUERY
    SELECT controls."Id",
           controls."FacilityId",
           controls."WorldId",
           controls."Timestamp",
           regions."FacilityName",
           controls."IsCapture",
           controls."OldFactionId",
           controls."NewFactionId",
           controls."ZoneId",
           regions."Id",
           regions."FacilityTypeId",
           regions."FacilityType"
      FROM "FacilityControl" AS controls
        INNER JOIN CurrentMapRegions AS regions
          on controls."FacilityId" = regions."FacilityId"
      WHERE (i_worldId IS NULL
            OR i_worldId = controls."WorldId")
        AND (i_facilityId IS NULL
            OR i_facilityId = controls."FacilityId")
      ORDER BY controls."Timestamp" DESC
      LIMIT CASE WHEN i_rowLimit IS NULL THEN 10
                 ELSE LEAST(i_rowLimit, 100::smallint) END;

END;
$BODY$