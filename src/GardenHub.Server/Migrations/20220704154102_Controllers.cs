using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardenHub.Server.Migrations
{
    public partial class Controllers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SensorControllerId",
                table: "Sensors",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "SensorController",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ControllerId = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorController", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_SensorControllerId",
                table: "Sensors",
                column: "SensorControllerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sensors_SensorController_SensorControllerId",
                table: "Sensors",
                column: "SensorControllerId",
                principalTable: "SensorController",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sensors_SensorController_SensorControllerId",
                table: "Sensors");

            migrationBuilder.DropTable(
                name: "SensorController");

            migrationBuilder.DropIndex(
                name: "IX_Sensors_SensorControllerId",
                table: "Sensors");

            migrationBuilder.DropColumn(
                name: "SensorControllerId",
                table: "Sensors");
        }
    }
}
