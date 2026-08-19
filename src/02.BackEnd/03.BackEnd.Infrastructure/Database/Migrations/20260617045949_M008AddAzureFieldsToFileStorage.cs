using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M008AddAzureFieldsToFileStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlobName",
                schema: "EBVL",
                table: "FileStorages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContainerName",
                schema: "EBVL",
                table: "FileStorages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecureUrl",
                schema: "EBVL",
                table: "FileStorages",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlobName",
                schema: "EBVL",
                table: "FileStorages");

            migrationBuilder.DropColumn(
                name: "ContainerName",
                schema: "EBVL",
                table: "FileStorages");

            migrationBuilder.DropColumn(
                name: "SecureUrl",
                schema: "EBVL",
                table: "FileStorages");
        }
    }
}
