using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VibeSync.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxSpaces = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StripePriceId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StripeCustomerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StripeSubscriptionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentPeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPlan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPlan_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPlan_Plan_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Plan",
                columns: new[] { "Id", "CreatedAt", "MaxSpaces", "Name", "Price", "StripePriceId" },
                values: new object[,]
                {
                    { new Guid("0b4f8c3d-a685-4d45-9c0e-3f85bc56ec15"), new DateTime(2025, 4, 10, 20, 51, 41, 236, DateTimeKind.Local).AddTicks(8126), 1, "Free", 0m, null },
                    { new Guid("3da77f60-87c2-4f63-9fdc-e3d33b186d05"), new DateTime(2025, 4, 10, 20, 51, 41, 238, DateTimeKind.Local).AddTicks(9116), 5, "Basic", 29.99m, "price_1RBlW7QTScSq3LFhzyNd64Ky" },
                    { new Guid("4d66ec50-fca2-4a18-972d-75683e9e2f14"), new DateTime(2025, 4, 10, 20, 51, 41, 238, DateTimeKind.Local).AddTicks(9147), 20, "Professional", 49.99m, "price_1RBnZrQTScSq3LFhSrkv57nG" },
                    { new Guid("692fa6dd-6ff1-4227-a49d-cff32643dcae"), new DateTime(2025, 4, 10, 20, 51, 41, 238, DateTimeKind.Local).AddTicks(9152), 2147483647, "Premium", 99.99m, "price_1RBnZrQTScSq3LFh5imaf4Jk" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPlan_PlanId",
                table: "UserPlan",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPlan_UserId",
                table: "UserPlan",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPlan");

            migrationBuilder.DropTable(
                name: "Plan");
        }
    }
}
