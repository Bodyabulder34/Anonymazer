
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Anonymazer.Migrations
{
    public partial class GeneralizeDetectionStatistics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Добавляем новое поле, пока допускающее NULL.
            migrationBuilder.AddColumn<string>(
                name: "DetectedCounts",
                table: "AnonymizationHistory",
                type: "jsonb",
                nullable: true);

            // 2. Переносим старую статистику в JSONB.
            migrationBuilder.Sql(
                """
                UPDATE "AnonymizationHistory"
                SET "DetectedCounts" = jsonb_build_object(
                    'EMAIL', "EmailsFound",
                    'PHONE', "PhonesFound",
                    'PASSPORT', "PassportsFound"
                );
                """);

            // 3. Делаем новое поле обязательным.
            migrationBuilder.AlterColumn<string>(
                name: "DetectedCounts",
                table: "AnonymizationHistory",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            // 4. Только теперь удаляем старые поля.
            migrationBuilder.DropColumn(
                name: "EmailsFound",
                table: "AnonymizationHistory");

            migrationBuilder.DropColumn(
                name: "PhonesFound",
                table: "AnonymizationHistory");

            migrationBuilder.DropColumn(
                name: "PassportsFound",
                table: "AnonymizationHistory");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Восстанавливаем старые столбцы.
            migrationBuilder.AddColumn<int>(
                name: "EmailsFound",
                table: "AnonymizationHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PhonesFound",
                table: "AnonymizationHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PassportsFound",
                table: "AnonymizationHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Возвращаем значения из JSONB в старые столбцы.
            migrationBuilder.Sql(
                """
                UPDATE "AnonymizationHistory"
                SET
                    "EmailsFound" = COALESCE(("DetectedCounts"->>'EMAIL')::integer, 0),
                    "PhonesFound" = COALESCE(("DetectedCounts"->>'PHONE')::integer, 0),
                    "PassportsFound" = COALESCE(("DetectedCounts"->>'PASSPORT')::integer, 0);
                """);

            migrationBuilder.DropColumn(
                name: "DetectedCounts",
                table: "AnonymizationHistory");
        }
    }
}
