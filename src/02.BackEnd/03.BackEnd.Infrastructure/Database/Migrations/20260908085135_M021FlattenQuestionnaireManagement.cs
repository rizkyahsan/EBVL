using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M021FlattenQuestionnaireManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireRules_QuestionnaireVersions_QuestionnaireVersionId",
                schema: "EBVL",
                table: "QuestionnaireRules");

            migrationBuilder.DropForeignKey(
                name: "FK_Questionnaires_QuestionnaireVersions_PublishedVersionId",
                schema: "EBVL",
                table: "Questionnaires");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireSections_QuestionnaireVersions_QuestionnaireVersionId",
                schema: "EBVL",
                table: "QuestionnaireSections");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireSubmissions_QuestionnaireVersions_QuestionnaireVersionId",
                schema: "EBVL",
                table: "QuestionnaireSubmissions");

            migrationBuilder.Sql(
                """
                ;WITH RankedVersions AS (
                    SELECT v.[Id], v.[QuestionnaireId],
                           ROW_NUMBER() OVER (
                               PARTITION BY v.[QuestionnaireId]
                               ORDER BY CASE WHEN v.[Status] = 'Draft' THEN 0 WHEN v.[IsActive] = 1 THEN 1 ELSE 2 END, v.[Version] DESC
                           ) AS [Rank]
                    FROM [EBVL].[QuestionnaireVersions] v
                    WHERE v.[IsDeleted] = 0
                )
                UPDATE s
                SET s.[Code] = CONCAT(LEFT(s.[Code], 12), '_HIST_', REPLACE(CONVERT(varchar(36), s.[Id]), '-', '')),
                    s.[IsDeleted] = 1,
                    s.[QuestionnaireVersionId] = v.[QuestionnaireId]
                FROM [EBVL].[QuestionnaireSections] s
                INNER JOIN [EBVL].[QuestionnaireVersions] v ON v.[Id] = s.[QuestionnaireVersionId]
                LEFT JOIN RankedVersions selected ON selected.[Id] = v.[Id] AND selected.[Rank] = 1
                WHERE selected.[Id] IS NULL;

                UPDATE s
                SET s.[QuestionnaireVersionId] = v.[QuestionnaireId]
                FROM [EBVL].[QuestionnaireSections] s
                INNER JOIN [EBVL].[QuestionnaireVersions] v ON v.[Id] = s.[QuestionnaireVersionId];

                ;WITH RankedVersions AS (
                    SELECT v.[Id], v.[QuestionnaireId],
                           ROW_NUMBER() OVER (
                               PARTITION BY v.[QuestionnaireId]
                               ORDER BY CASE WHEN v.[Status] = 'Draft' THEN 0 WHEN v.[IsActive] = 1 THEN 1 ELSE 2 END, v.[Version] DESC
                           ) AS [Rank]
                    FROM [EBVL].[QuestionnaireVersions] v
                    WHERE v.[IsDeleted] = 0
                )
                UPDATE r
                SET r.[QuestionnaireVersionId] = v.[QuestionnaireId],
                    r.[IsDeleted] = CASE WHEN selected.[Id] IS NULL THEN 1 ELSE r.[IsDeleted] END
                FROM [EBVL].[QuestionnaireRules] r
                INNER JOIN [EBVL].[QuestionnaireVersions] v ON v.[Id] = r.[QuestionnaireVersionId]
                LEFT JOIN RankedVersions selected ON selected.[Id] = v.[Id] AND selected.[Rank] = 1;

                UPDATE s
                SET s.[QuestionnaireVersionId] = v.[QuestionnaireId]
                FROM [EBVL].[QuestionnaireSubmissions] s
                INNER JOIN [EBVL].[QuestionnaireVersions] v ON v.[Id] = s.[QuestionnaireVersionId];
                """);

            migrationBuilder.DropTable(
                name: "QuestionnaireVersions",
                schema: "EBVL");

            migrationBuilder.DropIndex(
                name: "IX_Questionnaires_PublishedVersionId",
                schema: "EBVL",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "PublishedVersionId",
                schema: "EBVL",
                table: "Questionnaires");

            migrationBuilder.RenameColumn(
                name: "QuestionnaireVersionId",
                schema: "EBVL",
                table: "QuestionnaireSubmissions",
                newName: "QuestionnaireId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionnaireSubmissions_QuestionnaireVersionId",
                schema: "EBVL",
                table: "QuestionnaireSubmissions",
                newName: "IX_QuestionnaireSubmissions_QuestionnaireId");

            migrationBuilder.RenameColumn(
                name: "QuestionnaireVersionId",
                schema: "EBVL",
                table: "QuestionnaireSections",
                newName: "QuestionnaireId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionnaireSections_QuestionnaireVersionId_Code",
                schema: "EBVL",
                table: "QuestionnaireSections",
                newName: "IX_QuestionnaireSections_QuestionnaireId_Code");

            migrationBuilder.RenameColumn(
                name: "QuestionnaireVersionId",
                schema: "EBVL",
                table: "QuestionnaireRules",
                newName: "QuestionnaireId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionnaireRules_QuestionnaireVersionId_SourceQuestionId_TargetQuestionId",
                schema: "EBVL",
                table: "QuestionnaireRules",
                newName: "IX_QuestionnaireRules_QuestionnaireId_SourceQuestionId_TargetQuestionId");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "EBVL",
                table: "Questionnaires",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "EBVL",
                table: "Questionnaires",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionnaireRules_Questionnaires_QuestionnaireId",
                schema: "EBVL",
                table: "QuestionnaireRules",
                column: "QuestionnaireId",
                principalSchema: "EBVL",
                principalTable: "Questionnaires",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionnaireSections_Questionnaires_QuestionnaireId",
                schema: "EBVL",
                table: "QuestionnaireSections",
                column: "QuestionnaireId",
                principalSchema: "EBVL",
                principalTable: "Questionnaires",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionnaireSubmissions_Questionnaires_QuestionnaireId",
                schema: "EBVL",
                table: "QuestionnaireSubmissions",
                column: "QuestionnaireId",
                principalSchema: "EBVL",
                principalTable: "Questionnaires",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireRules_Questionnaires_QuestionnaireId",
                schema: "EBVL",
                table: "QuestionnaireRules");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireSections_Questionnaires_QuestionnaireId",
                schema: "EBVL",
                table: "QuestionnaireSections");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireSubmissions_Questionnaires_QuestionnaireId",
                schema: "EBVL",
                table: "QuestionnaireSubmissions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "EBVL",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "EBVL",
                table: "Questionnaires");

            migrationBuilder.RenameColumn(
                name: "QuestionnaireId",
                schema: "EBVL",
                table: "QuestionnaireSubmissions",
                newName: "QuestionnaireVersionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionnaireSubmissions_QuestionnaireId",
                schema: "EBVL",
                table: "QuestionnaireSubmissions",
                newName: "IX_QuestionnaireSubmissions_QuestionnaireVersionId");

            migrationBuilder.RenameColumn(
                name: "QuestionnaireId",
                schema: "EBVL",
                table: "QuestionnaireSections",
                newName: "QuestionnaireVersionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionnaireSections_QuestionnaireId_Code",
                schema: "EBVL",
                table: "QuestionnaireSections",
                newName: "IX_QuestionnaireSections_QuestionnaireVersionId_Code");

            migrationBuilder.RenameColumn(
                name: "QuestionnaireId",
                schema: "EBVL",
                table: "QuestionnaireRules",
                newName: "QuestionnaireVersionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionnaireRules_QuestionnaireId_SourceQuestionId_TargetQuestionId",
                schema: "EBVL",
                table: "QuestionnaireRules",
                newName: "IX_QuestionnaireRules_QuestionnaireVersionId_SourceQuestionId_TargetQuestionId");

            migrationBuilder.AddColumn<Guid>(
                name: "PublishedVersionId",
                schema: "EBVL",
                table: "Questionnaires",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QuestionnaireVersions",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true),
                    PublishedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionnaireVersions_Questionnaires_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalSchema: "EBVL",
                        principalTable: "Questionnaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaires_PublishedVersionId",
                schema: "EBVL",
                table: "Questionnaires",
                column: "PublishedVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireVersions_QuestionnaireId_Version",
                schema: "EBVL",
                table: "QuestionnaireVersions",
                columns: new[] { "QuestionnaireId", "Version" },
                unique: true);

            migrationBuilder.Sql(
                """
                INSERT INTO [EBVL].[QuestionnaireVersions]
                    ([Id], [QuestionnaireId], [Created], [CreatedBy], [IsActive], [IsDeleted], [Status], [Version])
                SELECT q.[Id], q.[Id], q.[Created], q.[CreatedBy], 1, 0, 'Published', 1
                FROM [EBVL].[Questionnaires] q;

                UPDATE [EBVL].[Questionnaires] SET [PublishedVersionId] = [Id];
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionnaireRules_QuestionnaireVersions_QuestionnaireVersionId",
                schema: "EBVL",
                table: "QuestionnaireRules",
                column: "QuestionnaireVersionId",
                principalSchema: "EBVL",
                principalTable: "QuestionnaireVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questionnaires_QuestionnaireVersions_PublishedVersionId",
                schema: "EBVL",
                table: "Questionnaires",
                column: "PublishedVersionId",
                principalSchema: "EBVL",
                principalTable: "QuestionnaireVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionnaireSections_QuestionnaireVersions_QuestionnaireVersionId",
                schema: "EBVL",
                table: "QuestionnaireSections",
                column: "QuestionnaireVersionId",
                principalSchema: "EBVL",
                principalTable: "QuestionnaireVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionnaireSubmissions_QuestionnaireVersions_QuestionnaireVersionId",
                schema: "EBVL",
                table: "QuestionnaireSubmissions",
                column: "QuestionnaireVersionId",
                principalSchema: "EBVL",
                principalTable: "QuestionnaireVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
