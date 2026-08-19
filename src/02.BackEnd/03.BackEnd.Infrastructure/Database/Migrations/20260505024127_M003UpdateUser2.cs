using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M003UpdateUser2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OtpUrl",
                schema: "EBVL",
                table: "Users",
                type: "nvarchar(2048)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)");

            migrationBuilder.AlterColumn<string>(
                name: "OtpSecret",
                schema: "EBVL",
                table: "Users",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)");

            migrationBuilder.AddColumn<Guid>(
                name: "IdentityUserId",
                schema: "EBVL",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LenderId",
                schema: "EBVL",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Users_LenderId",
                schema: "EBVL",
                table: "Users",
                column: "LenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Lenders_LenderId",
                schema: "EBVL",
                table: "Users",
                column: "LenderId",
                principalSchema: "EBVL",
                principalTable: "Lenders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Lenders_LenderId",
                schema: "EBVL",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_LenderId",
                schema: "EBVL",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                schema: "EBVL",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LenderId",
                schema: "EBVL",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "OtpUrl",
                schema: "EBVL",
                table: "Users",
                type: "nvarchar(2048)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OtpSecret",
                schema: "EBVL",
                table: "Users",
                type: "nvarchar(200)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true);
        }
    }
}
