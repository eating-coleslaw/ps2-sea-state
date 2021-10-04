CREATE OR REPLACE FUNCTION GetFacilityControlAttributedPlayers(
  i_facilityId int,
  i_timestamp timestamp without time zone,
  i_worldId smallint
)
  RETURNS TABLE (
    Id text,
    "Name" text,
    Faction_Id smallint,
    OutfitId text
  )
LANGUAGE plpgsql
AS
$BODY$
BEGIN

  RETURN QUERY
    SELECT characters."Id",
          characters."Name",
          characters."FactionId",
          characters."OutfitId"
      FROM "PlayerFacilityControl" AS controls
        LEFT OUTER JOIN "Character" as characters
          ON controls."CharacterId" = characters."Id"
      WHERE controls."FacilityId" = i_facilityId
        AND controls."Timestamp" = i_timestamp
        AND controls."WorldId" = i_worldId;

END;
$BODY$