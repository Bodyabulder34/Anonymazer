
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Anonymazer.Migrations
{
    /// <inheritdoc />
    public partial class AddAnonymizationJobs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnonymizationJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false),

                    UserId = table.Column<int>(
                        type: "integer",
                        nullable: false),

                    Status = table.Column<string>(
                        type: "text",
                        nullable: false),

                    OriginalText = table.Column<string>(
                        type: "text",
                        nullable: false),

                    AnonymizedText = table.Column<string>(
                        type: "text",
                        nullable: true),

                    CreatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false),

                    CompletedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true),

                    ErrorMessage = table.Column<string>(
                        type: "text",
                        nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_AnonymizationJobs",
                        x => x.Id);

                    table.ForeignKey(
                        name: "FK_AnonymizationJobs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnonymizationJobs_UserId",
                table: "AnonymizationJobs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnonymizationJobs");
        }
    }
}
