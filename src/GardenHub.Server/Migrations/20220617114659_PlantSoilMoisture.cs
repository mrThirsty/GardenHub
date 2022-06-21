using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardenHub.Server.Migrations
{
    public partial class PlantSoilMoisture : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "RequiredSoilMoisture",
                table: "Plant",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiredSoilMoisture",
                table: "Plant");
        }
    }
}
