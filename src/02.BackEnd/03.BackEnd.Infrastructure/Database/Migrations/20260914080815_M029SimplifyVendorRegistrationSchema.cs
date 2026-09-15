using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M029SimplifyVendorRegistrationSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExternalLogins_ExternalLoginLogs_ExternalLoginLogId",
                schema: "EBVL",
                table: "ExternalLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireAnswerFiles_FileStorages_FileStorageId",
                schema: "EBVL",
                table: "QuestionnaireAnswerFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireAnswers_QuestionnaireQuestionSnapshots_QuestionnaireQuestionId",
                schema: "EBVL",
                table: "QuestionnaireAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireAnswers_QuestionnaireSubmissions_QuestionnaireSubmissionId",
                schema: "EBVL",
                table: "QuestionnaireAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorRegistrationDocuments_VendorRegistrationDocumentRequirements_DocumentRequirementId",
                schema: "EBVL",
                table: "VendorRegistrationDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorRegistrations_Users_UserId",
                schema: "EBVL",
                table: "VendorRegistrations");

            migrationBuilder.DropTable(
                name: "ExternalLoginLogs",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "QuestionnaireAnswerOptions",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "QuestionnaireRuleSnapshots",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "VendorRegistrationDocumentRequirements",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "QuestionnaireOptionSnapshots",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "QuestionnaireQuestionSnapshots",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "QuestionnaireSectionSnapshots",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "QuestionnaireSubmissions",
                schema: "EBVL");

            migrationBuilder.DropIndex(
                name: "IX_VendorRegistrations_NormalizedSapVendorNumber",
                schema: "EBVL",
                table: "VendorRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_VendorRegistrations_UserId",
                schema: "EBVL",
                table: "VendorRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_VendorRegistrationDocuments_DocumentRequirementId",
                schema: "EBVL",
                table: "VendorRegistrationDocuments");

            migrationBuilder.DropIndex(
                name: "IX_VendorRegistrationDocuments_FileStorageId",
                schema: "EBVL",
                table: "VendorRegistrationDocuments");

            migrationBuilder.DropIndex(
                name: "IX_QuestionnaireAnswerFiles_FileStorageId",
                schema: "EBVL",
                table: "QuestionnaireAnswerFiles");

            migrationBuilder.DropIndex(
                name: "IX_ExternalLogins_ExternalLoginLogId",
                schema: "EBVL",
                table: "ExternalLogins");

            migrationBuilder.DropColumn(
                name: "DocumentRequirementId",
                schema: "EBVL",
                table: "VendorRegistrationDocuments");

            migrationBuilder.DropColumn(
                name: "ExternalLoginLogId",
                schema: "EBVL",
                table: "ExternalLogins");

            migrationBuilder.RenameColumn(
                name: "QuestionnaireSubmissionId",
                schema: "EBVL",
                table: "QuestionnaireAnswers",
                newName: "VendorRegistrationId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionnaireAnswers_QuestionnaireSubmissionId_QuestionnaireQuestionId",
                schema: "EBVL",
                table: "QuestionnaireAnswers",
                newName: "IX_QuestionnaireAnswers_VendorRegistrationId_QuestionnaireQuestionId");

            migrationBuilder.AlterColumn<string>(
                name: "Website",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "SapVendorNumber",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "ResumeTokenHash",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "varbinary(max)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(32)",
                oldMaxLength: 32);

            migrationBuilder.AlterColumn<string>(
                name: "RepresentativeName",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "PicPhoneNumber",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "PicEmail",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(320)",
                oldMaxLength: 320);

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedSapVendorNumber",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FactoryCountry",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "FactoryAddress",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<int>(
                name: "CompanyService",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyPhoneNumber",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyEmail",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(320)",
                oldMaxLength: 320);

            migrationBuilder.AlterColumn<string>(
                name: "BrandRepresentative",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<Guid>(
                name: "QuestionnaireId",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentDefinitionId",
                schema: "EBVL",
                table: "VendorRegistrationDocuments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsMandatory",
                schema: "EBVL",
                table: "VendorRegistrationDocuments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxSizeMb",
                schema: "EBVL",
                table: "VendorRegistrationDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "EBVL",
                table: "VendorRegistrationDocuments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                schema: "EBVL",
                table: "VendorRegistrationDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "OriginalFileName",
                schema: "EBVL",
                table: "QuestionnaireAnswerFiles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(260)",
                oldMaxLength: 260);

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                schema: "EBVL",
                table: "QuestionnaireAnswerFiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(320)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "EBVL",
                table: "QuestionnaireAnswerFiles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(320)");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                schema: "EBVL",
                table: "QuestionnaireAnswerFiles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "AttemptedAt",
                schema: "EBVL",
                table: "ExternalLogins",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FailureReason",
                schema: "EBVL",
                table: "ExternalLogins",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                schema: "EBVL",
                table: "ExternalLogins",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSuccess",
                schema: "EBVL",
                table: "ExternalLogins",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                schema: "EBVL",
                table: "ExternalLogins",
                type: "nvarchar(320)",
                maxLength: 320,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "VerifiedAt",
                schema: "EBVL",
                table: "ExternalLogins",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorRegistrations_QuestionnaireId",
                schema: "EBVL",
                table: "VendorRegistrations",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorRegistrations_UserId",
                schema: "EBVL",
                table: "VendorRegistrations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorRegistrationDocuments_DocumentDefinitionId",
                schema: "EBVL",
                table: "VendorRegistrationDocuments",
                column: "DocumentDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorRegistrationDocuments_FileStorageId",
                schema: "EBVL",
                table: "VendorRegistrationDocuments",
                column: "FileStorageId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireAnswerFiles_FileStorageId",
                schema: "EBVL",
                table: "QuestionnaireAnswerFiles",
                column: "FileStorageId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionnaireAnswerFiles_FileStorages_FileStorageId",
                schema: "EBVL",
                table: "QuestionnaireAnswerFiles",
                column: "FileStorageId",
                principalSchema: "EBVL",
                principalTable: "FileStorages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_QuestionnaireAnswers_VendorRegistrations_VendorRegistrationId",
                schema: "EBVL",
                table: "QuestionnaireAnswers",
                column: "VendorRegistrationId",
                principalSchema: "EBVL",
                principalTable: "VendorRegistrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorRegistrationDocuments_DocumentDefinitions_DocumentDefinitionId",
                schema: "EBVL",
                table: "VendorRegistrationDocuments",
                column: "DocumentDefinitionId",
                principalSchema: "EBVL",
                principalTable: "DocumentDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorRegistrations_Questionnaires_QuestionnaireId",
                schema: "EBVL",
                table: "VendorRegistrations",
                column: "QuestionnaireId",
                principalSchema: "EBVL",
                principalTable: "Questionnaires",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorRegistrations_Users_UserId",
                schema: "EBVL",
                table: "VendorRegistrations",
                column: "UserId",
                principalSchema: "EBVL",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireAnswerFiles_FileStorages_FileStorageId",
                schema: "EBVL",
                table: "QuestionnaireAnswerFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireAnswers_QuestionnaireQuestions_QuestionnaireQuestionId",
                schema: "EBVL",
                table: "QuestionnaireAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireAnswers_VendorRegistrations_VendorRegistrationId",
                schema: "EBVL",
                table: "QuestionnaireAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorRegistrationDocuments_DocumentDefinitions_DocumentDefinitionId",
                schema: "EBVL",
                table: "VendorRegistrationDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorRegistrations_Questionnaires_QuestionnaireId",
                schema: "EBVL",
                table: "VendorRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorRegistrations_Users_UserId",
                schema: "EBVL",
                table: "VendorRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_VendorRegistrations_QuestionnaireId",
                schema: "EBVL",
                table: "VendorRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_VendorRegistrations_UserId",
                schema: "EBVL",
                table: "VendorRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_VendorRegistrationDocuments_DocumentDefinitionId",
                schema: "EBVL",
                table: "VendorRegistrationDocuments");

            migrationBuilder.DropIndex(
                name: "IX_VendorRegistrationDocuments_FileStorageId",
                schema: "EBVL",
                table: "VendorRegistrationDocuments");

            migrationBuilder.DropIndex(
                name: "IX_QuestionnaireAnswerFiles_FileStorageId",
                schema: "EBVL",
                table: "QuestionnaireAnswerFiles");

            migrationBuilder.DropColumn(
                name: "QuestionnaireId",
                schema: "EBVL",
                table: "VendorRegistrations");

            migrationBuilder.DropColumn(
                name: "DocumentDefinitionId",
                schema: "EBVL",
                table: "VendorRegistrationDocuments");

            migrationBuilder.DropColumn(
                name: "IsMandatory",
                schema: "EBVL",
                table: "VendorRegistrationDocuments");

            migrationBuilder.DropColumn(
                name: "MaxSizeMb",
                schema: "EBVL",
                table: "VendorRegistrationDocuments");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "EBVL",
                table: "VendorRegistrationDocuments");

            migrationBuilder.DropColumn(
                name: "Order",
                schema: "EBVL",
                table: "VendorRegistrationDocuments");

            migrationBuilder.DropColumn(
                name: "AttemptedAt",
                schema: "EBVL",
                table: "ExternalLogins");

            migrationBuilder.DropColumn(
                name: "FailureReason",
                schema: "EBVL",
                table: "ExternalLogins");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                schema: "EBVL",
                table: "ExternalLogins");

            migrationBuilder.DropColumn(
                name: "IsSuccess",
                schema: "EBVL",
                table: "ExternalLogins");

            migrationBuilder.DropColumn(
                name: "Username",
                schema: "EBVL",
                table: "ExternalLogins");

            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                schema: "EBVL",
                table: "ExternalLogins");

            migrationBuilder.RenameColumn(
                name: "VendorRegistrationId",
                schema: "EBVL",
                table: "QuestionnaireAnswers",
                newName: "QuestionnaireSubmissionId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionnaireAnswers_VendorRegistrationId_QuestionnaireQuestionId",
                schema: "EBVL",
                table: "QuestionnaireAnswers",
                newName: "IX_QuestionnaireAnswers_QuestionnaireSubmissionId_QuestionnaireQuestionId");

            migrationBuilder.AlterColumn<string>(
                name: "Website",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SapVendorNumber",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "ResumeTokenHash",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "varbinary(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");

            migrationBuilder.AlterColumn<string>(
                name: "RepresentativeName",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PicPhoneNumber",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PicEmail",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "nvarchar(320)",
                maxLength: 320,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedSapVendorNumber",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FactoryCountry",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FactoryAddress",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyService",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyPhoneNumber",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyEmail",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "nvarchar(320)",
                maxLength: 320,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BrandRepresentative",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentRequirementId",
                schema: "EBVL",
                table: "VendorRegistrationDocuments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OriginalFileName",
                schema: "EBVL",
                table: "QuestionnaireAnswerFiles",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                schema: "EBVL",
                table: "QuestionnaireAnswerFiles",
                type: "nvarchar(320)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "EBVL",
                table: "QuestionnaireAnswerFiles",
                type: "nvarchar(320)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                schema: "EBVL",
                table: "QuestionnaireAnswerFiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "ExternalLoginLogId",
                schema: "EBVL",
                table: "ExternalLogins",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ExternalLoginLogs",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttemptedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    VerifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalLoginLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireSubmissions",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendorRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true),
                    QuestionnaireCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionnaireSubmissions_VendorRegistrations_VendorRegistrationId",
                        column: x => x.VendorRegistrationId,
                        principalSchema: "EBVL",
                        principalTable: "VendorRegistrations",
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
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    DocumentDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    MaxSizeMb = table.Column<int>(type: "int", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
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
                name: "QuestionnaireRuleSnapshots",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireSubmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SourceQuestionSnapshotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetQuestionSnapshotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
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
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    SourceSectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
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
                name: "QuestionnaireQuestionSnapshots",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireSectionSnapshotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnswerRule = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Hint = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Placeholder = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SourceQuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
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
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    SourceOptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "QuestionnaireAnswerOptions",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireAnswerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireOptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
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
                        name: "FK_QuestionnaireAnswerOptions_QuestionnaireOptionSnapshots_QuestionnaireOptionId",
                        column: x => x.QuestionnaireOptionId,
                        principalSchema: "EBVL",
                        principalTable: "QuestionnaireOptionSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_VendorRegistrationDocuments_DocumentRequirementId",
                schema: "EBVL",
                table: "VendorRegistrationDocuments",
                column: "DocumentRequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorRegistrationDocuments_FileStorageId",
                schema: "EBVL",
                table: "VendorRegistrationDocuments",
                column: "FileStorageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireAnswerFiles_FileStorageId",
                schema: "EBVL",
                table: "QuestionnaireAnswerFiles",
                column: "FileStorageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalLogins_ExternalLoginLogId",
                schema: "EBVL",
                table: "ExternalLogins",
                column: "ExternalLoginLogId",
                unique: true);

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
                name: "IX_QuestionnaireSubmissions_VendorRegistrationId",
                schema: "EBVL",
                table: "QuestionnaireSubmissions",
                column: "VendorRegistrationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorRegistrationDocumentRequirements_VendorRegistrationId_Code",
                schema: "EBVL",
                table: "VendorRegistrationDocumentRequirements",
                columns: new[] { "VendorRegistrationId", "Code" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalLogins_ExternalLoginLogs_ExternalLoginLogId",
                schema: "EBVL",
                table: "ExternalLogins",
                column: "ExternalLoginLogId",
                principalSchema: "EBVL",
                principalTable: "ExternalLoginLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionnaireAnswerFiles_FileStorages_FileStorageId",
                schema: "EBVL",
                table: "QuestionnaireAnswerFiles",
                column: "FileStorageId",
                principalSchema: "EBVL",
                principalTable: "FileStorages",
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
                name: "FK_QuestionnaireAnswers_QuestionnaireSubmissions_QuestionnaireSubmissionId",
                schema: "EBVL",
                table: "QuestionnaireAnswers",
                column: "QuestionnaireSubmissionId",
                principalSchema: "EBVL",
                principalTable: "QuestionnaireSubmissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorRegistrationDocuments_VendorRegistrationDocumentRequirements_DocumentRequirementId",
                schema: "EBVL",
                table: "VendorRegistrationDocuments",
                column: "DocumentRequirementId",
                principalSchema: "EBVL",
                principalTable: "VendorRegistrationDocumentRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorRegistrations_Users_UserId",
                schema: "EBVL",
                table: "VendorRegistrations",
                column: "UserId",
                principalSchema: "EBVL",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
