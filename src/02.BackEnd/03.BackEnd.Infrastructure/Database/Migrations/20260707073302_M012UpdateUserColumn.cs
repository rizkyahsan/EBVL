using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M012UpdateUserColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_IdentityUserId",
                schema: "EBVL",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "PhoneCode",
                schema: "EBVL",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneCode",
                schema: "EBVL",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdentityUserId",
                schema: "EBVL",
                table: "Users",
                column: "IdentityUserId",
                unique: true);
        }
    }
}
