using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VibeSync.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SpaceFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Suggestions_SpaceId",
                table: "Suggestions",
                column: "SpaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Suggestions_Spaces_SpaceId",
                table: "Suggestions",
                column: "SpaceId",
                principalTable: "Spaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Suggestions_Spaces_SpaceId",
                table: "Suggestions");

            migrationBuilder.DropIndex(
                name: "IX_Suggestions_SpaceId",
                table: "Suggestions");
        }
    }
}
