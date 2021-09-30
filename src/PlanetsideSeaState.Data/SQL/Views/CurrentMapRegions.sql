CREATE OR REPLACE VIEW CurrentMapRegions AS

  SELECT "Id",
        "FacilityId",
        "FacilityName",
        "FacilityTypeId",
        "FacilityType",
        "ZoneId"
    FROM "MapRegion"
    WHERE "IsDeprecated" = FALSE
      AND "IsCurrent" = TRUE