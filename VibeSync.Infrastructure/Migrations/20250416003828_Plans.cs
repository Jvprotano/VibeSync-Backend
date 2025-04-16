using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VibeSync.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Plans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPlan_AspNetUsers_UserId",
                table: "UserPlan");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPlan_Plan_PlanId",
                table: "UserPlan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPlan",
                table: "UserPlan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Plan",
                table: "Plan");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Plan");

            migrationBuilder.RenameTable(
                name: "UserPlan",
                newName: "UserPlans");

            migrationBuilder.RenameTable(
                name: "Plan",
                newName: "Plans");

            migrationBuilder.RenameIndex(
                name: "IX_UserPlan_UserId",
                table: "UserPlans",
                newName: "IX_UserPlans_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPlan_PlanId",
                table: "UserPlans",
                newName: "IX_UserPlans_PlanId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelAt",
                table: "UserPlans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPlans",
                table: "UserPlans",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Plans",
                table: "Plans",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPlans_AspNetUsers_UserId",
                table: "UserPlans",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPlans_Plans_PlanId",
                table: "UserPlans",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPlans_AspNetUsers_UserId",
                table: "UserPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPlans_Plans_PlanId",
                table: "UserPlans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPlans",
                table: "UserPlans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Plans",
                table: "Plans");

            migrationBuilder.DropColumn(
                name: "CancelAt",
                table: "UserPlans");

            migrationBuilder.RenameTable(
                name: "UserPlans",
                newName: "UserPlan");

            migrationBuilder.RenameTable(
                name: "Plans",
                newName: "Plan");

            migrationBuilder.RenameIndex(
                name: "IX_UserPlans_UserId",
                table: "UserPlan",
                newName: "IX_UserPlan_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPlans_PlanId",
                table: "UserPlan",
                newName: "IX_UserPlan_PlanId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Plan",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPlan",
                table: "UserPlan",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Plan",
                table: "Plan",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Plan",
                keyColumn: "Id",
                keyValue: new Guid("0b4f8c3d-a685-4d45-9c0e-3f85bc56ec15"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 10, 20, 51, 41, 236, DateTimeKind.Local).AddTicks(8126));

            migrationBuilder.UpdateData(
                table: "Plan",
                keyColumn: "Id",
                keyValue: new Guid("3da77f60-87c2-4f63-9fdc-e3d33b186d05"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 10, 20, 51, 41, 238, DateTimeKind.Local).AddTicks(9116));

            migrationBuilder.UpdateData(
                table: "Plan",
                keyColumn: "Id",
                keyValue: new Guid("4d66ec50-fca2-4a18-972d-75683e9e2f14"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 10, 20, 51, 41, 238, DateTimeKind.Local).AddTicks(9147));

            migrationBuilder.UpdateData(
                table: "Plan",
                keyColumn: "Id",
                keyValue: new Guid("692fa6dd-6ff1-4227-a49d-cff32643dcae"),
                column: "CreatedAt",
                value: new DateTime(2025, 4, 10, 20, 51, 41, 238, DateTimeKind.Local).AddTicks(9152));

            migrationBuilder.AddForeignKey(
                name: "FK_UserPlan_AspNetUsers_UserId",
                table: "UserPlan",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPlan_Plan_PlanId",
                table: "UserPlan",
                column: "PlanId",
                principalTable: "Plan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
