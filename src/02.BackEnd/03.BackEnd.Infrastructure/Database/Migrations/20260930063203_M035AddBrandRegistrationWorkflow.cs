using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M035AddBrandRegistrationWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkflowCases",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProcessType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DueAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowCases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BrandRegistrations",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowCaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerUsername = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    BrandName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Group = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FactoryCountry = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    SubmittedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrandRegistrations_WorkflowCases_WorkflowCaseId",
                        column: x => x.WorkflowCaseId,
                        principalSchema: "EBVL",
                        principalTable: "WorkflowCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowAssignments",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowCaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AssigneeUsername = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    AssignedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    AssignedBy = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    EndedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    EndReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowAssignments_WorkflowCases_WorkflowCaseId",
                        column: x => x.WorkflowCaseId,
                        principalSchema: "EBVL",
                        principalTable: "WorkflowCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowTransitions",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowCaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromStatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ToStatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActorUsername = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    ActorRole = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    OccurredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowTransitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowTransitions_WorkflowCases_WorkflowCaseId",
                        column: x => x.WorkflowCaseId,
                        principalSchema: "EBVL",
                        principalTable: "WorkflowCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BrandInvitations",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrandRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Recipient = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Cc = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentBy = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    SentAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Response = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RespondedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RespondedBy = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrandInvitations_BrandRegistrations_BrandRegistrationId",
                        column: x => x.BrandRegistrationId,
                        principalSchema: "EBVL",
                        principalTable: "BrandRegistrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrandInvitations_BrandRegistrationId_SentAt",
                schema: "EBVL",
                table: "BrandInvitations",
                columns: new[] { "BrandRegistrationId", "SentAt" });

            migrationBuilder.CreateIndex(
                name: "IX_BrandRegistrations_WorkflowCaseId",
                schema: "EBVL",
                table: "BrandRegistrations",
                column: "WorkflowCaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowAssignments_WorkflowCaseId",
                schema: "EBVL",
                table: "WorkflowAssignments",
                column: "WorkflowCaseId",
                unique: true,
                filter: "[EndedAt] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowCases_DueAt",
                schema: "EBVL",
                table: "WorkflowCases",
                column: "DueAt");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowCases_ProcessType_Status",
                schema: "EBVL",
                table: "WorkflowCases",
                columns: new[] { "ProcessType", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTransitions_WorkflowCaseId_OccurredAt",
                schema: "EBVL",
                table: "WorkflowTransitions",
                columns: new[] { "WorkflowCaseId", "OccurredAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrandInvitations",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "WorkflowAssignments",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "WorkflowTransitions",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "BrandRegistrations",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "WorkflowCases",
                schema: "EBVL");
        }
    }
}
