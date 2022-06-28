using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardenHub.Server.Migrations
{
    public partial class Reading : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SensorId",
                table: "Pot",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Reading",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PotId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reading", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reading_Pot_PotId",
                        column: x => x.PotId,
                        principalTable: "Pot",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pot_SensorId",
                table: "Pot",
                column: "SensorId");

            migrationBuilder.CreateIndex(
                name: "IX_Reading_PotId",
                table: "Reading",
                column: "PotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pot_Sensors_SensorId",
                table: "Pot",
                column: "SensorId",
                principalTable: "Sensors",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pot_Sensors_SensorId",
                table: "Pot");

            migrationBuilder.DropTable(
                name: "Reading");

            migrationBuilder.DropIndex(
                name: "IX_Pot_SensorId",
                table: "Pot");

            migrationBuilder.DropColumn(
                name: "SensorId",
                table: "Pot");
        }
    }
}
