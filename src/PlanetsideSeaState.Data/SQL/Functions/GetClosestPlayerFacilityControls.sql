CREATE OR REPLACE FUNCTION GetClosestPlayerFacilityControls(
  i_timestamp_start timestamp without time zone,
  i_timestamp_end timestamp without time zone,
  i_worldId smallint,
  i_zoneId bigint
)
  RETURNS TABLE (
    Character_Id text,
    Facility_Id int,
    "Timestamp" timestamp without time zone,
    Facility_Control_Id uuid,
    Zone_Id bigint,
    Time_Diff interval
  )
  LANGUAGE plpgsql
AS
$BODY$
DECLARE
  control_lookahead_interval interval := INTERVAL '1 minute';
  control_lookahead_end timestamp without time zone;

BEGIN
  control_lookahead_end := i_timestamp_end + control_lookahead_interval;

  RETURN QUERY

    SELECT event_chars.character_id AS Character_Id,
          MAX(CASE WHEN p_controls_before.time_diff IS NULL
                      THEN p_controls_after."FacilityId"
                    WHEN p_controls_after.time_diff IS NULL
                      THEN p_controls_before."FacilityId"
                    WHEN p_controls_before.time_diff > p_controls_after.time_diff
                      THEN p_controls_after."FacilityId"
                ELSE p_controls_before."FacilityId" END) AS Facility_Id,
          MAX(CASE WHEN p_controls_before.time_diff IS NULL
                      THEN p_controls_after."Timestamp"
                    WHEN p_controls_after.time_diff IS NULL
                      THEN p_controls_before."Timestamp"
                    WHEN p_controls_before.time_diff > p_controls_after.time_diff
                      THEN p_controls_after."Timestamp"
                ELSE p_controls_before."Timestamp" END) AS "Timestamp",
          CASE WHEN p_controls_before.time_diff IS NULL
                  THEN p_controls_after."FacilityControlId"
                WHEN p_controls_after.time_diff IS NULL
                  THEN p_controls_before."FacilityControlId"
                WHEN p_controls_before.time_diff > p_controls_after.time_diff
                  THEN p_controls_after."FacilityControlId"
            ELSE p_controls_before."FacilityControlId" END AS facility_control_id,
          MAX(CASE WHEN p_controls_before.time_diff IS NULL
                      THEN p_controls_after."ZoneId"
                    WHEN p_controls_after.time_diff IS NULL
                      THEN p_controls_before."ZoneId"
                    WHEN p_controls_before.time_diff > p_controls_after.time_diff
                      THEN p_controls_after."ZoneId"
                ELSE p_controls_before."ZoneId" END) AS zone_id,
          MAX(CASE WHEN p_controls_before.time_diff IS NULL
                      THEN p_controls_after.time_diff
                    WHEN p_controls_after.time_diff IS NULL
                      THEN p_controls_before.time_diff
                    WHEN p_controls_before.time_diff > p_controls_after.time_diff
                      THEN p_controls_after.time_diff
                ELSE p_controls_before.time_diff END) AS time_diff
      FROM (SELECT DISTINCT death."AttackerCharacterId"  AS character_id
              FROM "Death" death
              WHERE death."AttackerCharacterId" <> '0'
                AND death."Timestamp" BETWEEN i_timestamp_start AND i_timestamp_end
                AND death."WorldId" = i_worldId
                AND death."ZoneId" = i_zoneId

            UNION

            SELECT DISTINCT death."VictimCharacterId"  AS character_id
              FROM "Death" death
              WHERE death."AttackerCharacterId" <> '0'
                AND death."Timestamp" BETWEEN i_timestamp_start AND i_timestamp_end
                AND death."WorldId" = i_worldId
                AND death."ZoneId" = i_zoneId
            
            UNION

            SELECT exp_gains."CharacterId"  AS character_id
              FROM "ExperienceGain" exp_gains
              WHERE exp_gains."OtherId" IS NOT NULL
                AND exp_gains."Timestamp" BETWEEN i_timestamp_start AND i_timestamp_end
                AND exp_gains."WorldId" = i_worldId
                AND exp_gains."ZoneId" = i_zoneId
              
            UNION

            SELECT exp_gains."OtherId" AS character_id
              FROM "ExperienceGain" exp_gains
                INNER JOIN "Character" charB
                  ON exp_gains."OtherId" = charB."Id"
              WHERE exp_gains."OtherId" IS NOT NULL
                AND exp_gains."Timestamp" BETWEEN i_timestamp_start AND i_timestamp_end
                AND exp_gains."WorldId" = i_worldId
                AND exp_gains."ZoneId" = i_zoneId
              
            UNION

            SELECT destructions."AttackerCharacterId" AS character_id
              FROM "VehicleDestruction" destructions
                INNER JOIN "Character" charA
                  ON destructions."AttackerCharacterId" = charA."Id"
              WHERE destructions."AttackerCharacterId" IS NOT NULL
                AND destructions."VictimCharacterId" IS NOT NULL
                AND destructions."Timestamp" BETWEEN i_timestamp_start AND i_timestamp_end
                AND destructions."WorldId" = i_worldId
                AND destructions."ZoneId" = i_zoneId

            UNION

            SELECT destructions."VictimCharacterId" AS character_id
              FROM "VehicleDestruction" destructions
                INNER JOIN "Character" charB
                  ON destructions."VictimCharacterId" = charB."Id"
              WHERE destructions."AttackerCharacterId" IS NOT NULL
                AND destructions."VictimCharacterId" IS NOT NULL
                AND destructions."Timestamp" BETWEEN i_timestamp_start AND i_timestamp_end
                AND destructions."WorldId" = i_worldId
                AND destructions."ZoneId" = i_zoneId
            
            ORDER BY character_id DESC) AS event_chars
        LEFT OUTER JOIN (SELECT p_controls_a."FacilityId",
                                p_controls_a."Timestamp",
                                p_controls_a."CharacterId",
                                p_controls_a."FacilityControlId",
                                p_controls_a."ZoneId",
                                (i_timestamp_end - p_controls_a."Timestamp") AS time_diff
                          FROM "PlayerFacilityControl" AS p_controls_a
                            INNER JOIN (SELECT p_controls."FacilityId",
                                                MAX(p_controls."Timestamp") AS "Timestamp",
                                                p_controls."CharacterId"
                                          FROM "PlayerFacilityControl" p_controls
                                          WHERE p_controls."Timestamp" BETWEEN i_timestamp_start AND i_timestamp_end
                                          GROUP BY p_controls."FacilityId", p_controls."CharacterId" ) AS max_time
                              ON p_controls_a."CharacterId" = max_time."CharacterId"
                                  AND p_controls_a."FacilityId" = max_time."FacilityId"
                                  AND p_controls_a."Timestamp" = max_time."Timestamp") AS p_controls_before
          ON event_chars.character_id = p_controls_before."CharacterId"
        LEFT OUTER JOIN (SELECT p_controls_a."FacilityId",
                                p_controls_a."Timestamp",
                                p_controls_a."CharacterId",
                                p_controls_a."FacilityControlId",
                                p_controls_a."ZoneId",
                                (p_controls_a."Timestamp" - i_timestamp_end) AS time_diff
                          FROM "PlayerFacilityControl" AS p_controls_a
                            INNER JOIN (SELECT p_controls."FacilityId",
                                                MIN(p_controls."Timestamp") AS "Timestamp",
                                                p_controls."CharacterId"
                                          FROM "PlayerFacilityControl" p_controls
                                          WHERE p_controls."Timestamp" BETWEEN i_timestamp_end AND '2021-10-06T16:49:47'
                                          GROUP BY p_controls."FacilityId", p_controls."CharacterId" ) AS min_time
                              ON p_controls_a."CharacterId" = min_time."CharacterId"
                                  AND p_controls_a."FacilityId" = min_time."FacilityId"
                                  AND p_controls_a."Timestamp" = min_time."Timestamp") AS p_controls_after
          ON event_chars.character_id = p_controls_after."CharacterId"
      WHERE p_controls_after."FacilityId" IS NOT NULL
        OR p_controls_before."FacilityId" IS NOT NULL
      GROUP BY event_chars.character_id, facility_control_id
      ORDER BY character_id ASC;

END;
$BODY$
