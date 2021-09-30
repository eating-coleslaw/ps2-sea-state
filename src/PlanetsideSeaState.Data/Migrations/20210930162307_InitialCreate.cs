﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PlanetsideSeaState.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Character",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsOnline = table.Column<bool>(type: "boolean", nullable: false),
                    OutfitId = table.Column<string>(type: "text", nullable: true),
                    OutfitAlias = table.Column<string>(type: "text", nullable: true),
                    OutfitAliasLower = table.Column<string>(type: "text", nullable: true),
                    FactionId = table.Column<short>(type: "smallint", nullable: false),
                    WorldId = table.Column<short>(type: "smallint", nullable: false),
                    BattleRank = table.Column<short>(type: "smallint", nullable: false),
                    PrestigeLevel = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Character", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Death",
                columns: table => new
                {
                    Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AttackerCharacterId = table.Column<string>(type: "text", nullable: false),
                    VictimCharacterId = table.Column<string>(type: "text", nullable: false),
                    DeathType = table.Column<int>(type: "integer", nullable: false),
                    AttackerLoadoutId = table.Column<short>(type: "smallint", nullable: true),
                    AttackerFactionId = table.Column<short>(type: "smallint", nullable: true),
                    VictimLoadoutId = table.Column<short>(type: "smallint", nullable: true),
                    VictimFactionId = table.Column<short>(type: "smallint", nullable: true),
                    IsHeadshot = table.Column<bool>(type: "boolean", nullable: false),
                    WeaponId = table.Column<int>(type: "integer", nullable: true),
                    AttackerVehicleId = table.Column<int>(type: "integer", nullable: true),
                    ZoneId = table.Column<long>(type: "bigint", nullable: true),
                    WorldId = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Death", x => new { x.Timestamp, x.AttackerCharacterId, x.VictimCharacterId });
                });

            migrationBuilder.CreateTable(
                name: "Experience",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Xp = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experience", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExperienceGain",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterId = table.Column<string>(type: "text", nullable: false),
                    ExperienceId = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    WorldId = table.Column<short>(type: "smallint", nullable: false),
                    ZoneId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    LoadoutId = table.Column<short>(type: "smallint", nullable: true),
                    OtherId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperienceGain", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FacilityControl",
                columns: table => new
                {
                    Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FacilityId = table.Column<int>(type: "integer", nullable: false),
                    WorldId = table.Column<short>(type: "smallint", nullable: false),
                    ControlType = table.Column<int>(type: "integer", nullable: false),
                    OldFactionId = table.Column<short>(type: "smallint", nullable: true),
                    NewFactionId = table.Column<short>(type: "smallint", nullable: true),
                    ZoneId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityControl", x => new { x.Timestamp, x.FacilityId, x.WorldId });
                });

            migrationBuilder.CreateTable(
                name: "FacilityLink",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FacilityIdA = table.Column<int>(type: "integer", nullable: false),
                    FacilityIdB = table.Column<int>(type: "integer", nullable: false),
                    ZoneId = table.Column<long>(type: "bigint", nullable: false),
                    Desription = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityLink", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FacilityType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MapRegion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    FacilityId = table.Column<int>(type: "integer", nullable: false),
                    FacilityName = table.Column<string>(type: "text", nullable: true),
                    FacilityTypeId = table.Column<int>(type: "integer", nullable: false),
                    FacilityType = table.Column<string>(type: "text", nullable: true),
                    ZoneId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeprecated = table.Column<bool>(type: "boolean", nullable: false),
                    IsCurrent = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapRegion", x => new { x.Id, x.FacilityId });
                });

            migrationBuilder.CreateTable(
                name: "PlayerFacilityCapture",
                columns: table => new
                {
                    CharacterId = table.Column<string>(type: "text", nullable: false),
                    FacilityId = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    WorldId = table.Column<short>(type: "smallint", nullable: false),
                    ZoneId = table.Column<long>(type: "bigint", nullable: false),
                    OutfitId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerFacilityCapture", x => new { x.Timestamp, x.CharacterId, x.FacilityId });
                });

            migrationBuilder.CreateTable(
                name: "PlayerFacilityDefend",
                columns: table => new
                {
                    CharacterId = table.Column<string>(type: "text", nullable: false),
                    FacilityId = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    WorldId = table.Column<short>(type: "smallint", nullable: false),
                    ZoneId = table.Column<long>(type: "bigint", nullable: false),
                    OutfitId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerFacilityDefend", x => new { x.Timestamp, x.CharacterId, x.FacilityId });
                });

            migrationBuilder.CreateTable(
                name: "PlayerLogin",
                columns: table => new
                {
                    CharacterId = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    WorldId = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerLogin", x => new { x.Timestamp, x.CharacterId });
                });

            migrationBuilder.CreateTable(
                name: "PlayerLogout",
                columns: table => new
                {
                    CharacterId = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    WorldId = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerLogout", x => new { x.Timestamp, x.CharacterId });
                });

            migrationBuilder.CreateTable(
                name: "UpdaterScheduler",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    LastUpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpdaterScheduler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleDestruction",
                columns: table => new
                {
                    Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AttackerCharacterId = table.Column<string>(type: "text", nullable: false),
                    VictimCharacterId = table.Column<string>(type: "text", nullable: false),
                    VictimVehicleId = table.Column<int>(type: "integer", nullable: false),
                    DeathType = table.Column<int>(type: "integer", nullable: false),
                    AttackerVehicleId = table.Column<int>(type: "integer", nullable: true),
                    WeaponId = table.Column<int>(type: "integer", nullable: true),
                    IsVehicleWeapon = table.Column<bool>(type: "boolean", nullable: true),
                    WorldId = table.Column<short>(type: "smallint", nullable: false),
                    ZoneId = table.Column<long>(type: "bigint", nullable: true),
                    FacilityId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleDestruction", x => new { x.Timestamp, x.AttackerCharacterId, x.VictimCharacterId, x.VictimVehicleId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExperienceGain_Timestamp_CharacterId_ExperienceId",
                table: "ExperienceGain",
                columns: new[] { "Timestamp", "CharacterId", "ExperienceId" });

            migrationBuilder.CreateIndex(
                name: "IX_ExperienceGain_Timestamp_WorldId_ExperienceId_ZoneId",
                table: "ExperienceGain",
                columns: new[] { "Timestamp", "WorldId", "ExperienceId", "ZoneId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Character");

            migrationBuilder.DropTable(
                name: "Death");

            migrationBuilder.DropTable(
                name: "Experience");

            migrationBuilder.DropTable(
                name: "ExperienceGain");

            migrationBuilder.DropTable(
                name: "FacilityControl");

            migrationBuilder.DropTable(
                name: "FacilityLink");

            migrationBuilder.DropTable(
                name: "FacilityType");

            migrationBuilder.DropTable(
                name: "MapRegion");

            migrationBuilder.DropTable(
                name: "PlayerFacilityCapture");

            migrationBuilder.DropTable(
                name: "PlayerFacilityDefend");

            migrationBuilder.DropTable(
                name: "PlayerLogin");

            migrationBuilder.DropTable(
                name: "PlayerLogout");

            migrationBuilder.DropTable(
                name: "UpdaterScheduler");

            migrationBuilder.DropTable(
                name: "VehicleDestruction");
        }
    }
}
