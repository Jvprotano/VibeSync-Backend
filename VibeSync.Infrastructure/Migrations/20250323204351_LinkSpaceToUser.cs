using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VibeSync.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LinkSpaceToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Spaces",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Spaces_UserId",
                table: "Spaces",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Spaces_AspNetUsers_UserId",
                table: "Spaces",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Spaces_AspNetUsers_UserId",
                table: "Spaces");

            migrationBuilder.DropIndex(
                name: "IX_Spaces_UserId",
                table: "Spaces");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Spaces");
        }
    }
}
