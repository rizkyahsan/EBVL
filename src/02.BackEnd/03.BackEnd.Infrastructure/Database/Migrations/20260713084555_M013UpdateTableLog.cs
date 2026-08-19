using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M013UpdateTableLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProjectLenderId",
                schema: "EBVL",
                table: "LogTransactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectStageId",
                schema: "EBVL",
                table: "LogTransactions",
                type: "uniqueidentifier",
                nullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_LogTransactions_ProjectLenders_ProjectLenderId",
                schema: "EBVL",
                table: "LogTransactions",
                column: "ProjectLenderId",
                principalSchema: "EBVL",
                principalTable: "ProjectLenders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LogTransactions_ProjectStages_ProjectStageId",
                schema: "EBVL",
                table: "LogTransactions",
                column: "ProjectStageId",
                principalSchema: "EBVL",
                principalTable: "ProjectStages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogTransactions_ProjectLenders_ProjectLenderId",
                schema: "EBVL",
                table: "LogTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_LogTransactions_ProjectStages_ProjectStageId",
                schema: "EBVL",
                table: "LogTransactions");

            migrationBuilder.DropIndex(
                name: "IX_LogTransactions_ProjectLenderId",
                schema: "EBVL",
                table: "LogTransactions");

            migrationBuilder.DropIndex(
                name: "IX_LogTransactions_ProjectStageId",
                schema: "EBVL",
                table: "LogTransactions");

            migrationBuilder.DropColumn(
                name: "ProjectLenderId",
                schema: "EBVL",
                table: "LogTransactions");

            migrationBuilder.DropColumn(
                name: "ProjectStageId",
                schema: "EBVL",
                table: "LogTransactions");
        }
    }
}
