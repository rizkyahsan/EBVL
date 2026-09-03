using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M018AddVendorRegistrationQuestionnaireRuntime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VendorRegistrations",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompanyType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    SapVendorNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NormalizedSapVendorNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CompanyEmail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    PicEmail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    CompanyPhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PicPhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Website = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CompanyService = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    FactoryCountry = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FactoryAddress = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    BrandRepresentative = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AdditionalBrandsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRepresentativeInIndonesia = table.Column<bool>(type: "bit", nullable: false),
                    RepresentativeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ResumeTokenHash = table.Column<byte[]>(type: "varbinary(32)", maxLength: 32, nullable: false),
                    ResumeTokenExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SubmittedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorRegistrations_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "EBVL",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireSubmissions",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendorRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionnaireSubmissions_QuestionnaireVersions_QuestionnaireVersionId",
                        column: x => x.QuestionnaireVersionId,
                        principalSchema: "EBVL",
                        principalTable: "QuestionnaireVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionnaireSubmissions_VendorRegistrations_VendorRegistrationId",
                        column: x => x.VendorRegistrationId,
                        principalSchema: "EBVL",
                        principalTable: "VendorRegistrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorRegistrationDocuments",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendorRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileStorageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DefinitionKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Length = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorRegistrationDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorRegistrationDocuments_FileStorages_FileStorageId",
                        column: x => x.FileStorageId,
                        principalSchema: "EBVL",
                        principalTable: "FileStorages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorRegistrationDocuments_VendorRegistrations_VendorRegistrationId",
                        column: x => x.VendorRegistrationId,
                        principalSchema: "EBVL",
                        principalTable: "VendorRegistrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireAnswers",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireSubmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireQuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TextValue = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    IntegerValue = table.Column<long>(type: "bigint", nullable: true),
                    DecimalValue = table.Column<decimal>(type: "decimal(38,10)", precision: 38, scale: 10, nullable: true),
                    DateValue = table.Column<DateOnly>(type: "date", nullable: true),
                    BooleanValue = table.Column<bool>(type: "bit", nullable: true),
                    JsonValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionnaireAnswers_QuestionnaireQuestions_QuestionnaireQuestionId",
                        column: x => x.QuestionnaireQuestionId,
                        principalSchema: "EBVL",
                        principalTable: "QuestionnaireQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionnaireAnswers_QuestionnaireSubmissions_QuestionnaireSubmissionId",
                        column: x => x.QuestionnaireSubmissionId,
                        principalSchema: "EBVL",
                        principalTable: "QuestionnaireSubmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireAnswerFiles",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendorRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireAnswerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireQuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileStorageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Length = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireAnswerFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionnaireAnswerFiles_FileStorages_FileStorageId",
                        column: x => x.FileStorageId,
                        principalSchema: "EBVL",
                        principalTable: "FileStorages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionnaireAnswerFiles_QuestionnaireAnswers_QuestionnaireAnswerId",
                        column: x => x.QuestionnaireAnswerId,
                        principalSchema: "EBVL",
                        principalTable: "QuestionnaireAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireAnswerOptions",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireAnswerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireOptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireAnswerOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionnaireAnswerOptions_QuestionnaireAnswers_QuestionnaireAnswerId",
                        column: x => x.QuestionnaireAnswerId,
                        principalSchema: "EBVL",
                        principalTable: "QuestionnaireAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionnaireAnswerOptions_QuestionnaireOptions_QuestionnaireOptionId",
                        column: x => x.QuestionnaireOptionId,
                        principalSchema: "EBVL",
                        principalTable: "QuestionnaireOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireAnswerFiles_FileStorageId",
                schema: "EBVL",
                table: "QuestionnaireAnswerFiles",
                column: "FileStorageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireAnswerFiles_QuestionnaireAnswerId",
                schema: "EBVL",
                table: "QuestionnaireAnswerFiles",
                column: "QuestionnaireAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireAnswerOptions_QuestionnaireAnswerId_QuestionnaireOptionId",
                schema: "EBVL",
                table: "QuestionnaireAnswerOptions",
                columns: new[] { "QuestionnaireAnswerId", "QuestionnaireOptionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireAnswerOptions_QuestionnaireOptionId",
                schema: "EBVL",
                table: "QuestionnaireAnswerOptions",
                column: "QuestionnaireOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireAnswers_QuestionnaireQuestionId",
                schema: "EBVL",
                table: "QuestionnaireAnswers",
                column: "QuestionnaireQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireAnswers_QuestionnaireSubmissionId_QuestionnaireQuestionId",
                schema: "EBVL",
                table: "QuestionnaireAnswers",
                columns: new[] { "QuestionnaireSubmissionId", "QuestionnaireQuestionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireSubmissions_QuestionnaireVersionId",
                schema: "EBVL",
                table: "QuestionnaireSubmissions",
                column: "QuestionnaireVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireSubmissions_VendorRegistrationId",
                schema: "EBVL",
                table: "QuestionnaireSubmissions",
                column: "VendorRegistrationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorRegistrationDocuments_FileStorageId",
                schema: "EBVL",
                table: "VendorRegistrationDocuments",
                column: "FileStorageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorRegistrationDocuments_VendorRegistrationId_DefinitionKey",
                schema: "EBVL",
                table: "VendorRegistrationDocuments",
                columns: new[] { "VendorRegistrationId", "DefinitionKey" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_VendorRegistrations_NormalizedSapVendorNumber",
                schema: "EBVL",
                table: "VendorRegistrations",
                column: "NormalizedSapVendorNumber",
                unique: true,
                filter: "[NormalizedSapVendorNumber] IS NOT NULL AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_VendorRegistrations_UserId",
                schema: "EBVL",
                table: "VendorRegistrations",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL AND [IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionnaireAnswerFiles",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "QuestionnaireAnswerOptions",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "VendorRegistrationDocuments",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "QuestionnaireAnswers",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "QuestionnaireSubmissions",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "VendorRegistrations",
                schema: "EBVL");
        }
    }
}
