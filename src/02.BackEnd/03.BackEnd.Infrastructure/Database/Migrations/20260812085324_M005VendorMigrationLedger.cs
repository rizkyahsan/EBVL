using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M005VendorMigrationLedger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Migration");

            migrationBuilder.CreateTable(
                name: "Crosswalks",
                schema: "Migration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceTable = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetTable = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TargetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RunId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crosswalks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Quarantines",
                schema: "Migration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RunId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceTable = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quarantines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rows",
                schema: "Migration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RunId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceTable = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Outcome = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Processed = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Runs",
                schema: "Migration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceDatabase = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Started = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Completed = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Imported = table.Column<int>(type: "int", nullable: false),
                    Quarantined = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Runs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Crosswalks_SourceTable_SourceId",
                schema: "Migration",
                table: "Crosswalks",
                columns: new[] { "SourceTable", "SourceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quarantines_RunId_SourceTable_SourceId",
                schema: "Migration",
                table: "Quarantines",
                columns: new[] { "RunId", "SourceTable", "SourceId" });

            migrationBuilder.CreateIndex(
                name: "IX_Rows_RunId_SourceTable_SourceId",
                schema: "Migration",
                table: "Rows",
                columns: new[] { "RunId", "SourceTable", "SourceId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Crosswalks",
                schema: "Migration");

            migrationBuilder.DropTable(
                name: "Quarantines",
                schema: "Migration");

            migrationBuilder.DropTable(
                name: "Rows",
                schema: "Migration");

            migrationBuilder.DropTable(
                name: "Runs",
                schema: "Migration");
        }
    }
}
