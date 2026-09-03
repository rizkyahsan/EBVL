using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M016RemoveLegacyModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogTransactions",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "ProjectAttachments",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "ProjectFiles",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "ProjectLenderHistories",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "ProjectLenderReqFiles",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "Statuses",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "ProjectLenderReqs",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "ProjectReqs",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "ProjectLenders",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "ProjectStages",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "Projects",
                schema: "EBVL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FinanceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true),
                    Objective = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statuses",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(3)", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Table = table.Column<string>(type: "nvarchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectFiles",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    FileStorageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectFiles_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "EBVL",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectLenders",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    FileStorageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(300)", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectLenders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectLenders_Lenders_LenderId",
                        column: x => x.LenderId,
                        principalSchema: "EBVL",
                        principalTable: "Lenders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectLenders_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "EBVL",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectStages",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DueDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectStages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectStages_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "EBVL",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LogTransactions",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectLenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProjectStageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogTransactions_ProjectLenders_ProjectLenderId",
                        column: x => x.ProjectLenderId,
                        principalSchema: "EBVL",
                        principalTable: "ProjectLenders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LogTransactions_ProjectStages_ProjectStageId",
                        column: x => x.ProjectStageId,
                        principalSchema: "EBVL",
                        principalTable: "ProjectStages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LogTransactions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "EBVL",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectAttachments",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectStageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileStorageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    SortNo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectAttachments_ProjectStages_ProjectStageId",
                        column: x => x.ProjectStageId,
                        principalSchema: "EBVL",
                        principalTable: "ProjectStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectAttachments_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "EBVL",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectLenderReqs",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectLenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectStageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectLenderReqs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectLenderReqs_ProjectLenders_ProjectLenderId",
                        column: x => x.ProjectLenderId,
                        principalSchema: "EBVL",
                        principalTable: "ProjectLenders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectLenderReqs_ProjectStages_ProjectStageId",
                        column: x => x.ProjectStageId,
                        principalSchema: "EBVL",
                        principalTable: "ProjectStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectLenderReqs_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "EBVL",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectReqs",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectStageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    SortNo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectReqs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectReqs_ProjectStages_ProjectStageId",
                        column: x => x.ProjectStageId,
                        principalSchema: "EBVL",
                        principalTable: "ProjectStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectReqs_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "EBVL",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectLenderHistories",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectLenderReqId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectLenderHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectLenderHistories_ProjectLenderReqs_ProjectLenderReqId",
                        column: x => x.ProjectLenderReqId,
                        principalSchema: "EBVL",
                        principalTable: "ProjectLenderReqs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectLenderHistories_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "EBVL",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectLenderReqFiles",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectLenderReqId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectReqId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    FileStorageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectLenderReqFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectLenderReqFiles_ProjectLenderReqs_ProjectLenderReqId",
                        column: x => x.ProjectLenderReqId,
                        principalSchema: "EBVL",
                        principalTable: "ProjectLenderReqs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectLenderReqFiles_ProjectReqs_ProjectReqId",
                        column: x => x.ProjectReqId,
                        principalSchema: "EBVL",
                        principalTable: "ProjectReqs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectLenderReqFiles_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "EBVL",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogTransactions_ProjectId",
                schema: "EBVL",
                table: "LogTransactions",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_LogTransactions_ProjectLenderId",
                schema: "EBVL",
                table: "LogTransactions",
                column: "ProjectLenderId");

            migrationBuilder.CreateIndex(
                name: "IX_LogTransactions_ProjectStageId",
                schema: "EBVL",
                table: "LogTransactions",
                column: "ProjectStageId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAttachments_ProjectId",
                schema: "EBVL",
                table: "ProjectAttachments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAttachments_ProjectStageId",
                schema: "EBVL",
                table: "ProjectAttachments",
                column: "ProjectStageId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFiles_ProjectId",
                schema: "EBVL",
                table: "ProjectFiles",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLenderHistories_ProjectId",
                schema: "EBVL",
                table: "ProjectLenderHistories",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLenderHistories_ProjectLenderReqId",
                schema: "EBVL",
                table: "ProjectLenderHistories",
                column: "ProjectLenderReqId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLenderReqFiles_ProjectId",
                schema: "EBVL",
                table: "ProjectLenderReqFiles",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLenderReqFiles_ProjectLenderReqId",
                schema: "EBVL",
                table: "ProjectLenderReqFiles",
                column: "ProjectLenderReqId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLenderReqFiles_ProjectReqId",
                schema: "EBVL",
                table: "ProjectLenderReqFiles",
                column: "ProjectReqId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLenderReqs_ProjectId",
                schema: "EBVL",
                table: "ProjectLenderReqs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLenderReqs_ProjectLenderId",
                schema: "EBVL",
                table: "ProjectLenderReqs",
                column: "ProjectLenderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLenderReqs_ProjectStageId",
                schema: "EBVL",
                table: "ProjectLenderReqs",
                column: "ProjectStageId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLenders_LenderId",
                schema: "EBVL",
                table: "ProjectLenders",
                column: "LenderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLenders_ProjectId",
                schema: "EBVL",
                table: "ProjectLenders",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectReqs_ProjectId",
                schema: "EBVL",
                table: "ProjectReqs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectReqs_ProjectStageId",
                schema: "EBVL",
                table: "ProjectReqs",
                column: "ProjectStageId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStages_ProjectId",
                schema: "EBVL",
                table: "ProjectStages",
                column: "ProjectId");
        }
    }
}
