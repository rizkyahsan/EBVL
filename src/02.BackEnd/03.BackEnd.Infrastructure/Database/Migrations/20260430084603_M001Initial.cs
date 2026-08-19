using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M001Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "EBVL");

            migrationBuilder.CreateTable(
                name: "ApiCalls",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ServiceProvider = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ServiceCategory = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    RequestUrl = table.Column<string>(type: "nvarchar(2000)", nullable: false),
                    RequestMethod = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    RequestParameters = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseStatusCode = table.Column<int>(type: "int", nullable: false),
                    ResponseHeaders = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(4000)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiCalls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Audits",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActionType = table.Column<int>(type: "int", nullable: false),
                    ActionName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Configurations",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(1000)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(3)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true),
                    OriginalFileName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    StoredFileName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    FileContentType = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplates",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Module = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    DefaultTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefaultCc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileStorages",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    IsFileStorage = table.Column<bool>(type: "bit", nullable: false),
                    FileData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    FileHash = table.Column<string>(type: "nvarchar(64)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true),
                    OriginalFileName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    StoredFileName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    FileContentType = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileStorages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lenders",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(300)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Website = table.Column<string>(type: "nvarchar(2048)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lenders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PublicHolidays",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(2)", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(300)", nullable: false),
                    LocalName = table.Column<string>(type: "nvarchar(300)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicHolidays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statuses",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Table = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(3)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", nullable: true),
                    OtpSecret = table.Column<string>(type: "nvarchar(6)", nullable: false),
                    OtpUrl = table.Column<string>(type: "nvarchar(2048)", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    OtpLogin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OtpLoginExpired = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastLoginDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    OtpChangePassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OtpChangePasswordExpired = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastModifiedPasswordDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectLenders",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(300)", nullable: false),
                    FileStorageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
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
                    Level = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DueDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
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
                name: "ProjectAttachments",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectStageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortNo = table.Column<int>(type: "int", nullable: false),
                    FileStorageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
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
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
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
                    Name = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortNo = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
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
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Modified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(320)", nullable: true)
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
                    ProjectReqId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectLenderReqId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileStorageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(320)", nullable: false),
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiCalls",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "Audits",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "Configurations",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "Countries",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "Documents",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "EmailTemplates",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "FileStorages",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "ProjectAttachments",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "ProjectLenderHistories",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "ProjectLenderReqFiles",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "PublicHolidays",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "Statuses",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "Users",
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
                name: "Lenders",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "Projects",
                schema: "EBVL");
        }
    }
}
