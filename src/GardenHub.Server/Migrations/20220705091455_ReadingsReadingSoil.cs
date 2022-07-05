using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardenHub.Server.Migrations
{
    public partial class ReadingsReadingSoil : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "SoilMoistureReading",
                table: "Reading",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoilMoistureReading",
                table: "Reading");
        }
    }
}
