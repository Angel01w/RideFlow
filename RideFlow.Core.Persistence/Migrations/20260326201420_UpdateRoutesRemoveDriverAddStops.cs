using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RideFlow.Core.Persistence.Migrations
{
    public partial class UpdateRoutesRemoveDriverAddStops : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Stops",
                table: "Routes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stops",
                table: "Routes");
        }
    }
}