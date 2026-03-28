using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RideFlow.Core.Persistence.Migrations
{
    public partial class AddDriverToRoutes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = 'FK_Routes_Drivers_DriverId'
                )
                BEGIN
                    ALTER TABLE [Routes]
                    ADD CONSTRAINT [FK_Routes_Drivers_DriverId]
                    FOREIGN KEY ([DriverId]) REFERENCES [Drivers]([Id]) ON DELETE CASCADE;
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = 'FK_Routes_Drivers_DriverId'
                )
                BEGIN
                    ALTER TABLE [Routes]
                    DROP CONSTRAINT [FK_Routes_Drivers_DriverId];
                END
            ");
        }
    }
}