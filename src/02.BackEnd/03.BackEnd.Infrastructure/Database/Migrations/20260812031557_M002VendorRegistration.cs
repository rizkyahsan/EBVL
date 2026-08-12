using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M002VendorRegistration : Migration
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
                    SapVendorNumber = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(250)", nullable: false),
                    CompanyEmail = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    PicEmail = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    CompanyPhone = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    PicPhone = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    Website = table.Column<string>(type: "nvarchar(2048)", nullable: true),
                    CompanyService = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    FactoryCountry = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    FactoryAddress = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    BrandRepresentative = table.Column<string>(type: "nvarchar(250)", nullable: false),
                    CompanyStatus = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    HasRepresentativeInIndonesia = table.Column<bool>(type: "bit", nullable: false),
                    IndonesiaRepresentativeName = table.Column<string>(type: "nvarchar(250)", nullable: true),
                    BrandRegistrationLetterFileName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    CompanyProfileFileName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    ProductCatalogFileName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    ProjectExperienceFileName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    TaxCardFileName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    MainCertificateFileName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorRegistrations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VendorAccounts",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    PasswordSalt = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    VendorRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorAccounts_VendorRegistrations_VendorRegistrationId",
                        column: x => x.VendorRegistrationId,
                        principalSchema: "EBVL",
                        principalTable: "VendorRegistrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VendorAccounts_EmailAddress",
                schema: "EBVL",
                table: "VendorAccounts",
                column: "EmailAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorAccounts_VendorRegistrationId",
                schema: "EBVL",
                table: "VendorAccounts",
                column: "VendorRegistrationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorRegistrations_SapVendorNumber",
                schema: "EBVL",
                table: "VendorRegistrations",
                column: "SapVendorNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorRegistrations_Status",
                schema: "EBVL",
                table: "VendorRegistrations",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendorAccounts",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "VendorRegistrations",
                schema: "EBVL");
        }
    }
}
