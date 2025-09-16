using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VibeSync.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedPlanLimits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: new Guid("0b4f8c3d-a685-4d45-9c0e-3f85bc56ec15"),
                column: "MaxSpaces",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: new Guid("3da77f60-87c2-4f63-9fdc-e3d33b186d05"),
                column: "MaxSpaces",
                value: 10);

            migrationBuilder.UpdateData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: new Guid("4d66ec50-fca2-4a18-972d-75683e9e2f14"),
                column: "MaxSpaces",
                value: 30);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: new Guid("0b4f8c3d-a685-4d45-9c0e-3f85bc56ec15"),
                column: "MaxSpaces",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: new Guid("3da77f60-87c2-4f63-9fdc-e3d33b186d05"),
                column: "MaxSpaces",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: new Guid("4d66ec50-fca2-4a18-972d-75683e9e2f14"),
                column: "MaxSpaces",
                value: 20);
        }
    }
}
