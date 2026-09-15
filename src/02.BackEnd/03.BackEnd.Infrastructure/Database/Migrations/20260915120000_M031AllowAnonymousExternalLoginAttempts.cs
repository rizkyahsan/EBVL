using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations;

[DbContext(typeof(DatabaseService))]
[Migration("20260915120000_M031AllowAnonymousExternalLoginAttempts")]
public partial class M031AllowAnonymousExternalLoginAttempts : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_ExternalLogins_Users_UserId",
            schema: "EBVL",
            table: "ExternalLogins");

        migrationBuilder.AlterColumn<Guid>(
            name: "UserId",
            schema: "EBVL",
            table: "ExternalLogins",
            type: "uniqueidentifier",
            nullable: true,
            oldClrType: typeof(Guid),
            oldType: "uniqueidentifier");

        migrationBuilder.AddForeignKey(
            name: "FK_ExternalLogins_Users_UserId",
            schema: "EBVL",
            table: "ExternalLogins",
            column: "UserId",
            principalSchema: "EBVL",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        throw new NotSupportedException("Rollback would discard anonymous login audit records. Resolve those records explicitly before reverting this migration.");
    }
}
