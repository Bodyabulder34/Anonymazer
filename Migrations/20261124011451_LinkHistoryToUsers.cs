using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Anonymazer.Migrations
{
    /// <inheritdoc />
    public partial class LinkHistoryToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "AnonymizationHistory",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnonymizationHistory_UserId",
                table: "AnonymizationHistory",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnonymizationHistory_Users_UserId",
                table: "AnonymizationHistory",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnonymizationHistory_Users_UserId",
                table: "AnonymizationHistory");

            migrationBuilder.DropIndex(
                name: "IX_AnonymizationHistory_UserId",
                table: "AnonymizationHistory");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AnonymizationHistory");
        }
    }
}
