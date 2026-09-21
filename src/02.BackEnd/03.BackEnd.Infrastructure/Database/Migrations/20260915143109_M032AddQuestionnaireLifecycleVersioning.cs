using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M032AddQuestionnaireLifecycleVersioning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Questionnaires_Code",
                schema: "EBVL",
                table: "Questionnaires");

            migrationBuilder.AddColumn<Guid>(
                name: "PreviousVersionId",
                schema: "EBVL",
                table: "Questionnaires",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PublishedAt",
                schema: "EBVL",
                table: "Questionnaires",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublishedBy",
                schema: "EBVL",
                table: "Questionnaires",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "QuestionnaireSeriesId",
                schema: "EBVL",
                table: "Questionnaires",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "EBVL",
                table: "Questionnaires",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Version",
                schema: "EBVL",
                table: "Questionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                SET XACT_ABORT ON;

                UPDATE [EBVL].[Questionnaires]
                SET QuestionnaireSeriesId = Id, Version = 1, Status = 'Superseded'
                WHERE IsDeleted = 1;

                ;WITH Parsed AS
                (
                    SELECT Id, IsActive, Created,
                        CASE WHEN Code LIKE '%[_]V[0-9]%' AND TRY_CONVERT(int, RIGHT(Code, CHARINDEX('V_', REVERSE(Code)) - 1)) IS NOT NULL
                            THEN LEFT(Code, LEN(Code) - CHARINDEX('V_', REVERSE(Code)) - 1) ELSE Code END AS SeriesCode,
                        CASE WHEN Code LIKE '%[_]V[0-9]%' AND TRY_CONVERT(int, RIGHT(Code, CHARINDEX('V_', REVERSE(Code)) - 1)) IS NOT NULL
                            THEN TRY_CONVERT(int, RIGHT(Code, CHARINDEX('V_', REVERSE(Code)) - 1)) ELSE 1 END AS ParsedVersion
                    FROM [EBVL].[Questionnaires]
                    WHERE [IsDeleted] = 0
                ), Numbered AS
                (
                    SELECT *, FIRST_VALUE(Id) OVER (PARTITION BY SeriesCode ORDER BY ParsedVersion, Created, Id) AS SeriesId,
                        ROW_NUMBER() OVER (PARTITION BY SeriesCode ORDER BY ParsedVersion, Created, Id) AS SequentialVersion,
                        SUM(CASE WHEN IsActive = 1 THEN 1 ELSE 0 END) OVER (PARTITION BY SeriesCode) AS ActiveCount,
                        COUNT(*) OVER (PARTITION BY SeriesCode) AS SeriesCount
                    FROM Parsed
                )
                UPDATE q SET QuestionnaireSeriesId = n.SeriesId, Code = n.SeriesCode, Version = n.SequentialVersion,
                    Status = CASE
                        WHEN n.IsActive = 1 THEN 'Published'
                        WHEN n.SeriesCount = 1 AND EXISTS (SELECT 1 FROM [EBVL].[VendorRegistrations] vr WHERE vr.QuestionnaireId = q.Id AND vr.IsDeleted = 0) THEN 'Published'
                        WHEN n.SeriesCount = 1 THEN 'Draft'
                        ELSE 'Superseded' END,
                    PublishedAt = CASE WHEN n.IsActive = 1 OR (n.SeriesCount = 1 AND EXISTS (SELECT 1 FROM [EBVL].[VendorRegistrations] vr WHERE vr.QuestionnaireId = q.Id AND vr.IsDeleted = 0)) THEN COALESCE(q.Modified, q.Created) ELSE NULL END,
                    PublishedBy = CASE WHEN n.IsActive = 1 OR (n.SeriesCount = 1 AND EXISTS (SELECT 1 FROM [EBVL].[VendorRegistrations] vr WHERE vr.QuestionnaireId = q.Id AND vr.IsDeleted = 0)) THEN COALESCE(q.ModifiedBy, q.CreatedBy) ELSE NULL END
                FROM [EBVL].[Questionnaires] q INNER JOIN Numbered n ON n.Id = q.Id;

                IF EXISTS (SELECT 1 FROM [EBVL].[VendorRegistrations] WHERE IsDeleted = 0 AND QuestionnaireId IS NULL)
                    THROW 51001, 'Vendor registrations without a concrete questionnaire version were found. Repair these registrations from historical evidence before applying M032.', 1;

                IF EXISTS (
                    SELECT 1 FROM [EBVL].[Questionnaires] q
                    WHERE q.IsDeleted = 0
                    GROUP BY q.QuestionnaireSeriesId
                    HAVING SUM(CASE WHEN q.Status = 'Published' THEN 1 ELSE 0 END) > 1
                        OR SUM(CASE WHEN q.Status = 'Draft' THEN 1 ELSE 0 END) > 1)
                    THROW 51000, 'Ambiguous legacy questionnaire lifecycle data. Resolve duplicate active/draft versions before applying M032.', 1;

                IF EXISTS (
                    SELECT 1 FROM [EBVL].[Questionnaires] q
                    WHERE q.IsDeleted = 0
                    GROUP BY q.QuestionnaireSeriesId
                    HAVING COUNT(*) > 1
                        AND SUM(CASE WHEN q.Status IN ('Published', 'Draft') THEN 1 ELSE 0 END) = 0)
                    THROW 51002, 'Legacy questionnaire series without an active version was found. Choose the intended draft or published version before applying M032.', 1;

                ;WITH Chain AS
                (
                    SELECT Id, LAG(Id) OVER (PARTITION BY QuestionnaireSeriesId ORDER BY Version) AS PreviousId
                    FROM [EBVL].[Questionnaires]
                    WHERE IsDeleted = 0
                )
                UPDATE q SET PreviousVersionId = c.PreviousId
                FROM [EBVL].[Questionnaires] q INNER JOIN Chain c ON c.Id = q.Id;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaires_PreviousVersionId",
                schema: "EBVL",
                table: "Questionnaires",
                column: "PreviousVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaires_OneDraftPerSeries",
                schema: "EBVL",
                table: "Questionnaires",
                column: "QuestionnaireSeriesId",
                unique: true,
                filter: "[Status] = 'Draft' AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaires_OnePublishedPerSeries",
                schema: "EBVL",
                table: "Questionnaires",
                column: "QuestionnaireSeriesId",
                unique: true,
                filter: "[Status] = 'Published' AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaires_OneSeriesPerCode",
                schema: "EBVL",
                table: "Questionnaires",
                column: "Code",
                unique: true,
                filter: "[Version] = 1 AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaires_QuestionnaireSeriesId_Version",
                schema: "EBVL",
                table: "Questionnaires",
                columns: new[] { "QuestionnaireSeriesId", "Version" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Questionnaires_Questionnaires_PreviousVersionId",
                schema: "EBVL",
                table: "Questionnaires",
                column: "PreviousVersionId",
                principalSchema: "EBVL",
                principalTable: "Questionnaires",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questionnaires_Questionnaires_PreviousVersionId",
                schema: "EBVL",
                table: "Questionnaires");

            migrationBuilder.DropIndex(
                name: "IX_Questionnaires_PreviousVersionId",
                schema: "EBVL",
                table: "Questionnaires");

            migrationBuilder.DropIndex(
                name: "IX_Questionnaires_OneDraftPerSeries",
                schema: "EBVL",
                table: "Questionnaires");

            migrationBuilder.DropIndex(
                name: "IX_Questionnaires_OnePublishedPerSeries",
                schema: "EBVL",
                table: "Questionnaires");

            migrationBuilder.DropIndex(
                name: "IX_Questionnaires_OneSeriesPerCode",
                schema: "EBVL",
                table: "Questionnaires");

            migrationBuilder.DropIndex(
                name: "IX_Questionnaires_QuestionnaireSeriesId_Version",
                schema: "EBVL",
                table: "Questionnaires");

            migrationBuilder.Sql("""
                UPDATE [EBVL].[Questionnaires]
                SET [Code] = LEFT([Code], 50 - LEN(CONCAT('_V', [Version]))) + CONCAT('_V', [Version])
                WHERE [Version] > 1;
                """);

            migrationBuilder.DropColumn(
                name: "PreviousVersionId",
                schema: "EBVL",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "PublishedAt",
                schema: "EBVL",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "PublishedBy",
                schema: "EBVL",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "QuestionnaireSeriesId",
                schema: "EBVL",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "EBVL",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "EBVL",
                table: "Questionnaires");

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaires_Code",
                schema: "EBVL",
                table: "Questionnaires",
                column: "Code",
                unique: true);
        }
    }
}
