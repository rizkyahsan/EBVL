using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M034AddDocumentRequirementSetLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DocumentDefinitions_BusinessProcess_Code",
                schema: "EBVL",
                table: "DocumentDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_DocumentDefinitions_BusinessProcess_Name",
                schema: "EBVL",
                table: "DocumentDefinitions");

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentRequirementSetId",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentRequirementSetId",
                schema: "EBVL",
                table: "DocumentDefinitions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DocumentRequirementSets",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentRequirementSetSeriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessProcess = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PreviousVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PublishedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PublishedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentRequirementSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentRequirementSets_DocumentRequirementSets_PreviousVersionId",
                        column: x => x.PreviousVersionId,
                        principalSchema: "EBVL",
                        principalTable: "DocumentRequirementSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql("""
                DECLARE @Sets TABLE ([BusinessProcess] nvarchar(100) NOT NULL, [Id] uniqueidentifier NOT NULL);
                INSERT INTO @Sets ([BusinessProcess], [Id])
                SELECT processes.[BusinessProcess], NEWID()
                FROM (
                    SELECT DISTINCT [BusinessProcess] FROM [EBVL].[DocumentDefinitions]
                    UNION SELECT N'Vendor Registration'
                ) processes;

                INSERT INTO [EBVL].[DocumentRequirementSets]
                    ([Id], [DocumentRequirementSetSeriesId], [BusinessProcess], [Version], [Status], [PublishedAt], [PublishedBy], [IsDeleted], [Created], [CreatedBy])
                SELECT [Id], [Id], [BusinessProcess], 1, 'Publish', SYSUTCDATETIME(), 'EBVLSystem', 0, SYSUTCDATETIME(), 'EBVLSystem'
                FROM @Sets;

                UPDATE definitions
                SET [DocumentRequirementSetId] = sets.[Id]
                FROM [EBVL].[DocumentDefinitions] definitions
                INNER JOIN @Sets sets ON sets.[BusinessProcess] = definitions.[BusinessProcess];

                UPDATE registrations
                SET [DocumentRequirementSetId] = sets.[Id]
                FROM [EBVL].[VendorRegistrations] registrations
                CROSS JOIN @Sets sets
                WHERE sets.[BusinessProcess] = 'Vendor Registration';
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "DocumentRequirementSetId",
                schema: "EBVL",
                table: "VendorRegistrations",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DocumentRequirementSetId",
                schema: "EBVL",
                table: "DocumentDefinitions",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorRegistrations_DocumentRequirementSetId",
                schema: "EBVL",
                table: "VendorRegistrations",
                column: "DocumentRequirementSetId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDefinitions_DocumentRequirementSetId_Code",
                schema: "EBVL",
                table: "DocumentDefinitions",
                columns: new[] { "DocumentRequirementSetId", "Code" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDefinitions_DocumentRequirementSetId_Name",
                schema: "EBVL",
                table: "DocumentDefinitions",
                columns: new[] { "DocumentRequirementSetId", "Name" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentRequirementSets_DocumentRequirementSetSeriesId_Version",
                schema: "EBVL",
                table: "DocumentRequirementSets",
                columns: new[] { "DocumentRequirementSetSeriesId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentRequirementSets_OneDraftPerSeries",
                schema: "EBVL",
                table: "DocumentRequirementSets",
                column: "DocumentRequirementSetSeriesId",
                unique: true,
                filter: "[Status] = 'Draft' AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentRequirementSets_OnePublishPerSeries",
                schema: "EBVL",
                table: "DocumentRequirementSets",
                column: "DocumentRequirementSetSeriesId",
                unique: true,
                filter: "[Status] = 'Publish' AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentRequirementSets_OneSeriesPerProcess",
                schema: "EBVL",
                table: "DocumentRequirementSets",
                column: "BusinessProcess",
                unique: true,
                filter: "[Version] = 1 AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentRequirementSets_PreviousVersionId",
                schema: "EBVL",
                table: "DocumentRequirementSets",
                column: "PreviousVersionId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentDefinitions_DocumentRequirementSets_DocumentRequirementSetId",
                schema: "EBVL",
                table: "DocumentDefinitions",
                column: "DocumentRequirementSetId",
                principalSchema: "EBVL",
                principalTable: "DocumentRequirementSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorRegistrations_DocumentRequirementSets_DocumentRequirementSetId",
                schema: "EBVL",
                table: "VendorRegistrations",
                column: "DocumentRequirementSetId",
                principalSchema: "EBVL",
                principalTable: "DocumentRequirementSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentDefinitions_DocumentRequirementSets_DocumentRequirementSetId",
                schema: "EBVL",
                table: "DocumentDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorRegistrations_DocumentRequirementSets_DocumentRequirementSetId",
                schema: "EBVL",
                table: "VendorRegistrations");

            migrationBuilder.DropTable(
                name: "DocumentRequirementSets",
                schema: "EBVL");

            migrationBuilder.DropIndex(
                name: "IX_VendorRegistrations_DocumentRequirementSetId",
                schema: "EBVL",
                table: "VendorRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_DocumentDefinitions_DocumentRequirementSetId_Code",
                schema: "EBVL",
                table: "DocumentDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_DocumentDefinitions_DocumentRequirementSetId_Name",
                schema: "EBVL",
                table: "DocumentDefinitions");

            migrationBuilder.DropColumn(
                name: "DocumentRequirementSetId",
                schema: "EBVL",
                table: "VendorRegistrations");

            migrationBuilder.DropColumn(
                name: "DocumentRequirementSetId",
                schema: "EBVL",
                table: "DocumentDefinitions");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDefinitions_BusinessProcess_Code",
                schema: "EBVL",
                table: "DocumentDefinitions",
                columns: new[] { "BusinessProcess", "Code" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDefinitions_BusinessProcess_Name",
                schema: "EBVL",
                table: "DocumentDefinitions",
                columns: new[] { "BusinessProcess", "Name" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }
    }
}
