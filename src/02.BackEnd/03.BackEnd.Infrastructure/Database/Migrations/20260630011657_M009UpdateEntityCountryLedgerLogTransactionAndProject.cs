using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M009UpdateEntityCountryLedgerLogTransactionAndProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                schema: "EBVL",
                table: "Lenders");

            migrationBuilder.AddColumn<Guid>(
                name: "CountryId",
                schema: "EBVL",
                table: "Lenders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                schema: "EBVL",
                table: "Countries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneCode",
                schema: "EBVL",
                table: "Countries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Region",
                schema: "EBVL",
                table: "Countries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LogTransactions",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogTransactions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "EBVL",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lenders_CountryId",
                schema: "EBVL",
                table: "Lenders",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_LogTransactions_ProjectId",
                schema: "EBVL",
                table: "LogTransactions",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lenders_Countries_CountryId",
                schema: "EBVL",
                table: "Lenders",
                column: "CountryId",
                principalSchema: "EBVL",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lenders_Countries_CountryId",
                schema: "EBVL",
                table: "Lenders");

            migrationBuilder.DropTable(
                name: "LogTransactions",
                schema: "EBVL");

            migrationBuilder.DropIndex(
                name: "IX_Lenders_CountryId",
                schema: "EBVL",
                table: "Lenders");

            migrationBuilder.DropColumn(
                name: "CountryId",
                schema: "EBVL",
                table: "Lenders");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                schema: "EBVL",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "PhoneCode",
                schema: "EBVL",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "Region",
                schema: "EBVL",
                table: "Countries");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                schema: "EBVL",
                table: "Lenders",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "");
        }
    }
}
