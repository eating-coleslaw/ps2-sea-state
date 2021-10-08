using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.IO;

namespace PlanetsideSeaState.Data.Migrations
{
    public partial class AddGetClosestPlayerFacilityControlsFunction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Run View / Function / Stored Procedure scripts
            var basePath = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;

            // GetClosestPlayerFacilityControls View
            var mapRegionsViewSqlFile = "SQL/Functions/GetClosestPlayerFacilityControls.sql";
            var mapRegionsViewFilePath = Path.Combine(basePath, mapRegionsViewSqlFile);
            migrationBuilder.Sql(File.ReadAllText(mapRegionsViewFilePath));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
