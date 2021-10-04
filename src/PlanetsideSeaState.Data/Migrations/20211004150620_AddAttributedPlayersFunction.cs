using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.IO;

namespace PlanetsideSeaState.Data.Migrations
{
    public partial class AddAttributedPlayersFunction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOnline",
                table: "Character");

            // Run View / Function / Stored Procedure scripts
            var basePath = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;

            // CurrentMapRegions View
            var mapRegionsViewSqlFile = "SQL/Functions/GetFacilityControlAttributedPlayers.sql";
            var mapRegionsViewFilePath = Path.Combine(basePath, mapRegionsViewSqlFile);
            migrationBuilder.Sql(File.ReadAllText(mapRegionsViewFilePath));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOnline",
                table: "Character",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
