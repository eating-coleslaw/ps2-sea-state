CREATE OR REPLACE FUNCTION GetPlayerConnectionEvents(
  i_timestamp_start timestamp without time zone,
  i_timestamp_end timestamp without time zone,
  i_worldId smallint DEFAULT NULL,
  i_zoneId bigint DEFAULT NULL
)
  RETURNS TABLE (
    Acting_Character_Id text,
    Recipient_Character_Id text,
    "Timestamp" timestamp without time zone,
    Event_Type int,
    Acting_Faction_Id smallint,
    Recipient_Faction_Id smallint,
    Experience_Id int,
    Zone_Id bigint
  )
  LANGUAGE plpgsql
AS
$BODY$
DECLARE
  death_type int := 2;
  exp_gain_type int := 4;
  vehicle_destroy_type int := 10;

BEGIN

  RETURN QUERY
    
    SELECT death."AttackerCharacterId",
           death."VictimCharacterId",
           death."Timestamp",
           death_type AS "EventType",
           death."AttackerFactionId",
           death."VictimFactionId",
           NULL::int AS "ExperienceId",
           death."ZoneId"
      FROM "Death" death
      WHERE death."AttackerCharacterId" <> '0'
        AND death."Timestamp" BETWEEN i_timestamp_start AND i_timestamp_end
        AND (i_worldId IS NULL OR death."WorldId" = i_worldId)
        AND (i_zoneId IS NULL OR death."ZoneId" = i_zoneId)

    UNION

    SELECT exp_gains."CharacterId",
           exp_gains."OtherId",
           exp_gains."Timestamp",
           exp_gain_type AS "EventType",
           charA."FactionId",
           charB."FactionId",
           exp_gains."ExperienceId",
           exp_gains."ZoneId"
      FROM "ExperienceGain" exp_gains
        INNER JOIN "Character" charA
          ON exp_gains."CharacterId" = charA."Id"
        INNER JOIN "Character" charB
          ON exp_gains."OtherId" = charB."Id"
      WHERE exp_gains."OtherId" IS NOT NULL
        AND exp_gains."Timestamp" BETWEEN i_timestamp_start AND i_timestamp_end
        AND (i_worldId IS NULL OR exp_gains."WorldId" = i_worldId)
        AND (i_zoneId IS NULL OR exp_gains."ZoneId" = i_zoneId)
    
    UNION

    SELECT destructions."AttackerCharacterId",
           destructions."VictimCharacterId",
           destructions."Timestamp",
           vehicle_destroy_type AS "EventType",
           charA."FactionId",
           charB."FactionId",
           NULL::int AS "ExperienceId",
           destructions."ZoneId"
      FROM "VehicleDestruction" destructions
        INNER JOIN "Character" charA
          ON destructions."AttackerCharacterId" = charA."Id"
        INNER JOIN "Character" charB
          ON destructions."VictimCharacterId" = charB."Id"
      WHERE destructions."AttackerCharacterId" IS NOT NULL
        AND destructions."VictimCharacterId" IS NOT NULL
        AND destructions."Timestamp" BETWEEN i_timestamp_start AND i_timestamp_end
        AND (i_worldId IS NULL OR destructions."WorldId" = i_worldId)
        AND (i_zoneId IS NULL OR destructions."ZoneId" = i_zoneId)
    
      ORDER BY "Timestamp" ASC;

END;
$BODY$