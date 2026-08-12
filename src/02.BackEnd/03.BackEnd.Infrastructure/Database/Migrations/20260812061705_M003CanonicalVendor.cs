using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M003CanonicalVendor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorAccounts_VendorRegistrations_VendorRegistrationId",
                schema: "EBVL",
                table: "VendorAccounts");

            migrationBuilder.DropIndex(
                name: "IX_VendorAccounts_VendorRegistrationId",
                schema: "EBVL",
                table: "VendorAccounts");

            migrationBuilder.AddColumn<Guid>(
                name: "VendorId",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "VendorRegistrationId",
                schema: "EBVL",
                table: "VendorAccounts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "EBVL",
                table: "VendorAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "VendorId",
                schema: "EBVL",
                table: "VendorAccounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContactTypes",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTemplates",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VendorTypes",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VendorRegistrationDocuments",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendorRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    StoredFileName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    FileContentType = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    StorageFileId = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    ContentHash = table.Column<string>(type: "nvarchar(64)", nullable: true),
                    ValidUntil = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
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
                        name: "FK_VendorRegistrationDocuments_DocumentTemplates_DocumentTemplateId",
                        column: x => x.DocumentTemplateId,
                        principalSchema: "EBVL",
                        principalTable: "DocumentTemplates",
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
                name: "Vendors",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SapVendorNumber = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    TaxId = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(2048)", nullable: true),
                    VendorTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LegacyConfirmedStatus = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vendors_VendorTypes_VendorTypeId",
                        column: x => x.VendorTypeId,
                        principalSchema: "EBVL",
                        principalTable: "VendorTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VendorContacts",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(320)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorContacts_ContactTypes_ContactTypeId",
                        column: x => x.ContactTypeId,
                        principalSchema: "EBVL",
                        principalTable: "ContactTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorContacts_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalSchema: "EBVL",
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VendorDocuments",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    StoredFileName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    FileContentType = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    StorageFileId = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    ContentHash = table.Column<string>(type: "nvarchar(64)", nullable: true),
                    ValidUntil = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorDocuments_DocumentTemplates_DocumentTemplateId",
                        column: x => x.DocumentTemplateId,
                        principalSchema: "EBVL",
                        principalTable: "DocumentTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorDocuments_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalSchema: "EBVL",
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VendorRegistrations_VendorId",
                schema: "EBVL",
                table: "VendorRegistrations",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorAccounts_VendorId",
                schema: "EBVL",
                table: "VendorAccounts",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorAccounts_VendorRegistrationId",
                schema: "EBVL",
                table: "VendorAccounts",
                column: "VendorRegistrationId",
                unique: true,
                filter: "[VendorRegistrationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTemplates_Alias",
                schema: "EBVL",
                table: "DocumentTemplates",
                column: "Alias",
                unique: true,
                filter: "[Alias] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VendorContacts_ContactTypeId",
                schema: "EBVL",
                table: "VendorContacts",
                column: "ContactTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorContacts_VendorId",
                schema: "EBVL",
                table: "VendorContacts",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorDocuments_DocumentTemplateId",
                schema: "EBVL",
                table: "VendorDocuments",
                column: "DocumentTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorDocuments_VendorId_DocumentTemplateId",
                schema: "EBVL",
                table: "VendorDocuments",
                columns: new[] { "VendorId", "DocumentTemplateId" });

            migrationBuilder.CreateIndex(
                name: "IX_VendorRegistrationDocuments_DocumentTemplateId",
                schema: "EBVL",
                table: "VendorRegistrationDocuments",
                column: "DocumentTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorRegistrationDocuments_VendorRegistrationId_DocumentTemplateId",
                schema: "EBVL",
                table: "VendorRegistrationDocuments",
                columns: new[] { "VendorRegistrationId", "DocumentTemplateId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_Email",
                schema: "EBVL",
                table: "Vendors",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_SapVendorNumber",
                schema: "EBVL",
                table: "Vendors",
                column: "SapVendorNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_TaxId",
                schema: "EBVL",
                table: "Vendors",
                column: "TaxId",
                unique: true,
                filter: "[TaxId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_VendorTypeId",
                schema: "EBVL",
                table: "Vendors",
                column: "VendorTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorAccounts_VendorRegistrations_VendorRegistrationId",
                schema: "EBVL",
                table: "VendorAccounts",
                column: "VendorRegistrationId",
                principalSchema: "EBVL",
                principalTable: "VendorRegistrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorAccounts_Vendors_VendorId",
                schema: "EBVL",
                table: "VendorAccounts",
                column: "VendorId",
                principalSchema: "EBVL",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorRegistrations_Vendors_VendorId",
                schema: "EBVL",
                table: "VendorRegistrations",
                column: "VendorId",
                principalSchema: "EBVL",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorAccounts_VendorRegistrations_VendorRegistrationId",
                schema: "EBVL",
                table: "VendorAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorAccounts_Vendors_VendorId",
                schema: "EBVL",
                table: "VendorAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorRegistrations_Vendors_VendorId",
                schema: "EBVL",
                table: "VendorRegistrations");

            migrationBuilder.DropTable(
                name: "VendorContacts",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "VendorDocuments",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "VendorRegistrationDocuments",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "ContactTypes",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "Vendors",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "DocumentTemplates",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "VendorTypes",
                schema: "EBVL");

            migrationBuilder.DropIndex(
                name: "IX_VendorRegistrations_VendorId",
                schema: "EBVL",
                table: "VendorRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_VendorAccounts_VendorId",
                schema: "EBVL",
                table: "VendorAccounts");

            migrationBuilder.DropIndex(
                name: "IX_VendorAccounts_VendorRegistrationId",
                schema: "EBVL",
                table: "VendorAccounts");

            migrationBuilder.DropColumn(
                name: "VendorId",
                schema: "EBVL",
                table: "VendorRegistrations");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "EBVL",
                table: "VendorAccounts");

            migrationBuilder.DropColumn(
                name: "VendorId",
                schema: "EBVL",
                table: "VendorAccounts");

            migrationBuilder.AlterColumn<Guid>(
                name: "VendorRegistrationId",
                schema: "EBVL",
                table: "VendorAccounts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorAccounts_VendorRegistrationId",
                schema: "EBVL",
                table: "VendorAccounts",
                column: "VendorRegistrationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorAccounts_VendorRegistrations_VendorRegistrationId",
                schema: "EBVL",
                table: "VendorAccounts",
                column: "VendorRegistrationId",
                principalSchema: "EBVL",
                principalTable: "VendorRegistrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
