DROP FUNCTION IF EXISTS GetRecentFacilityControls(smallint,integer,smallint);

CREATE OR REPLACE FUNCTION GetRecentFacilityControls(
  i_worldId smallint DEFAULT NULL,
  i_facilityId int DEFAULT NULL,
  i_rowLimit smallint DEFAULT 20
)
  RETURNS TABLE (
    Id uuid,
    Facility_Id int,
    World_Id smallint,
    "Timestamp" timestamp without time zone,
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
        -- This join to FacilityControlPlayers filters out FacilityControl events
        -- that have no attributed players. This is intended to exclude uninteresting
        -- control events, like those from a continent locking or unlocking.
        INNER JOIN (SELECT players."FacilityId",
                           players."Timestamp",
                           players."WorldId"
                       FROM "PlayerFacilityControl" players
                       GROUP BY players."FacilityId",
                               players."Timestamp",
                               players."WorldId" ) AS playerControls
          ON controls."FacilityId" = playerControls."FacilityId"
             AND controls."Timestamp" = playerControls."Timestamp"
             AND controls."WorldId" = playerControls."WorldId"
      WHERE (i_worldId IS NULL
            OR i_worldId = controls."WorldId")
        AND (i_facilityId IS NULL
            OR i_facilityId = controls."FacilityId")
        AND regions."FacilityTypeId" <> 7 -- Warpgate
      ORDER BY controls."Timestamp" DESC
      LIMIT CASE WHEN i_rowLimit IS NULL THEN 10
                 ELSE LEAST(i_rowLimit, 100::smallint) END;

END;
$BODY$