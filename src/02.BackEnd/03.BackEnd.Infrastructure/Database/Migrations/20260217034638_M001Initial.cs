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
                name: "Users",
                schema: "EBVL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(320)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", nullable: true),
                    OtpSecret = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    OtpUrl = table.Column<string>(type: "nvarchar(2048)", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
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
                name: "PublicHolidays",
                schema: "EBVL");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "EBVL");
        }
    }
}
