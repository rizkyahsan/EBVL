using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M007UpdateFileStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFileStorage",
                schema: "EBVL",
                table: "FileStorages");

            migrationBuilder.AlterColumn<Guid>(
                name: "FileStorageId",
                schema: "EBVL",
                table: "ProjectLenders",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "StorageType",
                schema: "EBVL",
                table: "FileStorages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StorageType",
                schema: "EBVL",
                table: "FileStorages");

            migrationBuilder.AlterColumn<Guid>(
                name: "FileStorageId",
                schema: "EBVL",
                table: "ProjectLenders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFileStorage",
                schema: "EBVL",
                table: "FileStorages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
