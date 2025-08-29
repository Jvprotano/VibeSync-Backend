using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VibeSync.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductsPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: new Guid("3da77f60-87c2-4f63-9fdc-e3d33b186d05"),
                column: "StripePriceId",
                value: "price_1RUFekKwZHJc78R4mzW5tmvH");

            migrationBuilder.UpdateData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: new Guid("4d66ec50-fca2-4a18-972d-75683e9e2f14"),
                column: "StripePriceId",
                value: "price_1RUFekKwZHJc78R4pLnyhSFm");

            migrationBuilder.UpdateData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: new Guid("692fa6dd-6ff1-4227-a49d-cff32643dcae"),
                column: "StripePriceId",
                value: "price_1RUFekKwZHJc78R4wqXb9Lye");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: new Guid("3da77f60-87c2-4f63-9fdc-e3d33b186d05"),
                column: "StripePriceId",
                value: "price_1RBnZrQTScSq3LFhcJeN8pXH");

            migrationBuilder.UpdateData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: new Guid("4d66ec50-fca2-4a18-972d-75683e9e2f14"),
                column: "StripePriceId",
                value: "price_1RBnZrQTScSq3LFhSrkv57nG");

            migrationBuilder.UpdateData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: new Guid("692fa6dd-6ff1-4227-a49d-cff32643dcae"),
                column: "StripePriceId",
                value: "price_1RBnZrQTScSq3LFh5imaf4Jk");
        }
    }
}
