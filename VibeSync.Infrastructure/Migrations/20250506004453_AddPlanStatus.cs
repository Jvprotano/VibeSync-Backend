using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VibeSync.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserPlans");

            migrationBuilder.RenameColumn(
                name: "CancelAt",
                table: "UserPlans",
                newName: "CancellationDate");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "UserPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserPlans");

            migrationBuilder.RenameColumn(
                name: "CancellationDate",
                table: "UserPlans",
                newName: "CancelAt");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
