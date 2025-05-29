using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VibeSync.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: new Guid("3da77f60-87c2-4f63-9fdc-e3d33b186d05"),
                column: "StripePriceId",
                value: "price_1RBnZrQTScSq3LFhcJeN8pXH");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: new Guid("3da77f60-87c2-4f63-9fdc-e3d33b186d05"),
                column: "StripePriceId",
                value: "price_1RBlW7QTScSq3LFhzyNd64Ky");
        }
    }
}
