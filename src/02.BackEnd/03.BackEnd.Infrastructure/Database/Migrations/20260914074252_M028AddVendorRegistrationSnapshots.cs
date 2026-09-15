using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M028AddVendorRegistrationSnapshots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireAnswerOptions_QuestionnaireOptions_QuestionnaireOptionId",
                schema: "EBVL",
                table: "QuestionnaireAnswerOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireAnswers_QuestionnaireQuestions_QuestionnaireQuestionId",
                schema: "EBVL",
                table: "QuestionnaireAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireSubmissions_Questionnaires_QuestionnaireId",
                schema: "EBVL",
                table: "QuestionnaireSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_QuestionnaireSubmissions_QuestionnaireId",
                schema: "EBVL",
                table: "QuestionnaireSubmissions");

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentRequirementId",
                schema: "EBVL",
                table: "VendorRegistrationDocuments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuestionnaireCode",
                schema: "EBVL",
                table: "QuestionnaireSubmissions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                schema: "EBVL",
                table: "DocumentDefinitions",
                type: "nvarchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                schema: "EBVL",
                table: "DocumentDefinitions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                """
                UPDATE [EBVL].[DocumentDefinitions]
                SET [Code] = CONCAT('DOCUMENT-', CONVERT(varchar(36), [Id]))
                WHERE [Code] = '';

                MERGE [EBVL].[DocumentDefinitions] AS target
                USING (VALUES
                    ('BrandRegistrationLetter', 'Brand Registration Letter', 1),
                    ('CompanyProfile', 'Company Profile', 2),
                    ('ProductCatalog', 'Product Catalog', 3),
                    ('ProductExperienceList', 'Product Experience List', 4),
                    ('CompanyTaxCard', 'NPWP Perusahaan', 5),
                    ('PrimaryCertificate', 'Brand Cert/STP/Authorized', 6)
                ) AS source ([Code], [Name], [Order])
                ON target.[BusinessProcess] = 'Vendor Registration'
                    AND target.[Name] = source.[Name]
                    AND target.[IsDeleted] = 0
                WHEN MATCHED THEN
                    UPDATE SET target.[Code] = source.[Code], target.[Order] = source.[Order]
                WHEN NOT MATCHED THEN
                    INSERT ([Id], [Code], [BusinessProcess], [Name], [Order], [MaxSizeMb], [IsMandatory], [IsActive], [IsDeleted], [Created], [CreatedBy])
                    VALUES (NEWID(), source.[Code], 'Vendor Registration', source.[Name], source.[Order], 50, 1, 1, 0, SYSDATETIMEOFFSET(), 'EBVLSystem');
                """);

            migrationBuilder.CreateTable(
                name: "QuestionnaireRuleSnapshots",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireSubmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceQuestionSnapshotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetQuestionSnapshotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_QuestionnaireRuleSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionnaireRuleSnapshots_QuestionnaireSubmissions_QuestionnaireSubmissionId",
                        column: x => x.QuestionnaireSubmissionId,
                        principalSchema: "EBVL",
                        principalTable: "QuestionnaireSubmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireSectionSnapshots",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireSubmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceSectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireSectionSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionnaireSectionSnapshots_QuestionnaireSubmissions_QuestionnaireSubmissionId",
                        column: x => x.QuestionnaireSubmissionId,
                        principalSchema: "EBVL",
                        principalTable: "QuestionnaireSubmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorRegistrationDocumentRequirements",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendorRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    MaxSizeMb = table.Column<int>(type: "int", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorRegistrationDocumentRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorRegistrationDocumentRequirements_VendorRegistrations_VendorRegistrationId",
                        column: x => x.VendorRegistrationId,
                        principalSchema: "EBVL",
                        principalTable: "VendorRegistrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireQuestionSnapshots",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireSectionSnapshotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceQuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Hint = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Placeholder = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    AnswerRule = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireQuestionSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionnaireQuestionSnapshots_QuestionnaireSectionSnapshots_QuestionnaireSectionSnapshotId",
                        column: x => x.QuestionnaireSectionSnapshotId,
                        principalSchema: "EBVL",
                        principalTable: "QuestionnaireSectionSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireOptionSnapshots",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireQuestionSnapshotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceOptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_QuestionnaireOptionSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionnaireOptionSnapshots_QuestionnaireQuestionSnapshots_QuestionnaireQuestionSnapshotId",
                        column: x => x.QuestionnaireQuestionSnapshotId,
                        principalSchema: "EBVL",
                        principalTable: "QuestionnaireQuestionSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VendorRegistrationDocuments_DocumentRequirementId",
                schema: "EBVL",
                table: "VendorRegistrationDocuments",
                column: "DocumentRequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDefinitions_BusinessProcess_Code",
                schema: "EBVL",
                table: "DocumentDefinitions",
                columns: new[] { "BusinessProcess", "Code" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireOptionSnapshots_QuestionnaireQuestionSnapshotId",
                schema: "EBVL",
                table: "QuestionnaireOptionSnapshots",
                column: "QuestionnaireQuestionSnapshotId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireQuestionSnapshots_QuestionnaireSectionSnapshotId",
                schema: "EBVL",
                table: "QuestionnaireQuestionSnapshots",
                column: "QuestionnaireSectionSnapshotId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireRuleSnapshots_QuestionnaireSubmissionId",
                schema: "EBVL",
                table: "QuestionnaireRuleSnapshots",
                column: "QuestionnaireSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireSectionSnapshots_QuestionnaireSubmissionId",
                schema: "EBVL",
                table: "QuestionnaireSectionSnapshots",
                column: "QuestionnaireSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorRegistrationDocumentRequirements_VendorRegistrationId_Code",
                schema: "EBVL",
                table: "VendorRegistrationDocumentRequirements",
                columns: new[] { "VendorRegistrationId", "Code" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionnaireAnswerOptions_QuestionnaireOptionSnapshots_QuestionnaireOptionId",
                schema: "EBVL",
                table: "QuestionnaireAnswerOptions",
                column: "QuestionnaireOptionId",
                principalSchema: "EBVL",
                principalTable: "QuestionnaireOptionSnapshots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionnaireAnswers_QuestionnaireQuestionSnapshots_QuestionnaireQuestionId",
                schema: "EBVL",
                table: "QuestionnaireAnswers",
                column: "QuestionnaireQuestionId",
                principalSchema: "EBVL",
                principalTable: "QuestionnaireQuestionSnapshots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorRegistrationDocuments_VendorRegistrationDocumentRequirements_DocumentRequirementId",
                schema: "EBVL",
                table: "VendorRegistrationDocuments",
                column: "DocumentRequirementId",
                principalSchema: "EBVL",
                principalTable: "VendorRegistrationDocumentRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireAnswerOptions_QuestionnaireOptionSnapshots_QuestionnaireOptionId",
                schema: "EBVL",
                table: "QuestionnaireAnswerOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireAnswers_QuestionnaireQuestionSnapshots_QuestionnaireQuestionId",
                schema: "EBVL",
                table: "QuestionnaireAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorRegistrationDocuments_VendorRegistrationDocumentRequirements_DocumentRequirementId",
                schema: "EBVL",
                table: "VendorRegistrationDocuments");

            migrationBuilder.DropTable(
                name: "QuestionnaireOptionSnapshots",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "QuestionnaireRuleSnapshots",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "VendorRegistrationDocumentRequirements",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "QuestionnaireQuestionSnapshots",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "QuestionnaireSectionSnapshots",
                schema: "EBVL");

            migrationBuilder.DropIndex(
                name: "IX_VendorRegistrationDocuments_DocumentRequirementId",
                schema: "EBVL",
                table: "VendorRegistrationDocuments");

            migrationBuilder.DropIndex(
                name: "IX_DocumentDefinitions_BusinessProcess_Code",
                schema: "EBVL",
                table: "DocumentDefinitions");

            migrationBuilder.DropColumn(
                name: "DocumentRequirementId",
                schema: "EBVL",
                table: "VendorRegistrationDocuments");

            migrationBuilder.DropColumn(
                name: "QuestionnaireCode",
                schema: "EBVL",
                table: "QuestionnaireSubmissions");

            migrationBuilder.DropColumn(
                name: "Code",
                schema: "EBVL",
                table: "DocumentDefinitions");

            migrationBuilder.DropColumn(
                name: "Order",
                schema: "EBVL",
                table: "DocumentDefinitions");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireSubmissions_QuestionnaireId",
                schema: "EBVL",
                table: "QuestionnaireSubmissions",
                column: "QuestionnaireId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionnaireAnswerOptions_QuestionnaireOptions_QuestionnaireOptionId",
                schema: "EBVL",
                table: "QuestionnaireAnswerOptions",
                column: "QuestionnaireOptionId",
                principalSchema: "EBVL",
                principalTable: "QuestionnaireOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionnaireAnswers_QuestionnaireQuestions_QuestionnaireQuestionId",
                schema: "EBVL",
                table: "QuestionnaireAnswers",
                column: "QuestionnaireQuestionId",
                principalSchema: "EBVL",
                principalTable: "QuestionnaireQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
    }
}
