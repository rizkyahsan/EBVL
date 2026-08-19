using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M004UpdateUser3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastLoginDate",
                schema: "EBVL",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastModifiedPasswordDate",
                schema: "EBVL",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OtpChangePassword",
                schema: "EBVL",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OtpChangePasswordExpired",
                schema: "EBVL",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OtpLogin",
                schema: "EBVL",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OtpLoginExpired",
                schema: "EBVL",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastLoginDate",
                schema: "EBVL",
                table: "Users",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastModifiedPasswordDate",
                schema: "EBVL",
                table: "Users",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtpChangePassword",
                schema: "EBVL",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "OtpChangePasswordExpired",
                schema: "EBVL",
                table: "Users",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtpLogin",
                schema: "EBVL",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "OtpLoginExpired",
                schema: "EBVL",
                table: "Users",
                type: "datetimeoffset",
                nullable: true);
        }
    }
}
