using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M017AddQuestionnaireManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuestionnaireOptions",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireQuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireOptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireQuestions",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireSectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Hint = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Placeholder = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireQuestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireRules",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceQuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetQuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Operator = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Questionnaires",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    BusinessProcess = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    PublishedVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questionnaires", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireVersions",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PublishedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "QuestionnaireSections",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CompanyType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionnaireSections_QuestionnaireVersions_QuestionnaireVersionId",
                        column: x => x.QuestionnaireVersionId,
                        principalSchema: "EBVL",
                        principalTable: "QuestionnaireVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireOptions_QuestionnaireQuestionId_Code",
                schema: "EBVL",
                table: "QuestionnaireOptions",
                columns: new[] { "QuestionnaireQuestionId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireQuestions_QuestionnaireSectionId_Code",
                schema: "EBVL",
                table: "QuestionnaireQuestions",
                columns: new[] { "QuestionnaireSectionId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireRules_QuestionnaireVersionId_SourceQuestionId_TargetQuestionId",
                schema: "EBVL",
                table: "QuestionnaireRules",
                columns: new[] { "QuestionnaireVersionId", "SourceQuestionId", "TargetQuestionId" });

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaires_Code",
                schema: "EBVL",
                table: "Questionnaires",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaires_PublishedVersionId",
                schema: "EBVL",
                table: "Questionnaires",
                column: "PublishedVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireSections_QuestionnaireVersionId_Code",
                schema: "EBVL",
                table: "QuestionnaireSections",
                columns: new[] { "QuestionnaireVersionId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireVersions_QuestionnaireId_Version",
                schema: "EBVL",
                table: "QuestionnaireVersions",
                columns: new[] { "QuestionnaireId", "Version" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionnaireOptions_QuestionnaireQuestions_QuestionnaireQuestionId",
                schema: "EBVL",
                table: "QuestionnaireOptions",
                column: "QuestionnaireQuestionId",
                principalSchema: "EBVL",
                principalTable: "QuestionnaireQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionnaireQuestions_QuestionnaireSections_QuestionnaireSectionId",
                schema: "EBVL",
                table: "QuestionnaireQuestions",
                column: "QuestionnaireSectionId",
                principalSchema: "EBVL",
                principalTable: "QuestionnaireSections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questionnaires_QuestionnaireVersions_PublishedVersionId",
                schema: "EBVL",
                table: "Questionnaires");

            migrationBuilder.DropTable(
                name: "QuestionnaireOptions",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "QuestionnaireRules",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "QuestionnaireQuestions",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "QuestionnaireSections",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "QuestionnaireVersions",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "Questionnaires",
                schema: "EBVL");
        }
    }
}
