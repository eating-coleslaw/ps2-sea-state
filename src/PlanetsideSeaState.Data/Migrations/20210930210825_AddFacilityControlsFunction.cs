using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.IO;

namespace PlanetsideSeaState.Data.Migrations
{
    public partial class AddFacilityControlsFunction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var basePath = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;

            // CurrentMapRegions View
            var mapRegionsViewSqlFile = "SQL/Views/CurrentMapRegions.sql";
            var mapRegionsViewFilePath = Path.Combine(basePath, mapRegionsViewSqlFile);
            migrationBuilder.Sql(File.ReadAllText(mapRegionsViewFilePath));

            // GetRecentFacilityControls Function
            var recentControlsFunctionSqlFile = "SQL/Functions/GetRecentFacilityControls.sql";
            var recentControlsFunctionFilePath = Path.Combine(basePath, recentControlsFunctionSqlFile);
            migrationBuilder.Sql(File.ReadAllText(recentControlsFunctionFilePath));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
