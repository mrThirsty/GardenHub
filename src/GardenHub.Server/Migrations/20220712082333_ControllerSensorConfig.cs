using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardenHub.Server.Migrations
{
    public partial class ControllerSensorConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "Sensors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "SensorController",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ReportingInterval",
                table: "SensorController",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "Sensors");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "SensorController");

            migrationBuilder.DropColumn(
                name: "ReportingInterval",
                table: "SensorController");
        }
    }
}
