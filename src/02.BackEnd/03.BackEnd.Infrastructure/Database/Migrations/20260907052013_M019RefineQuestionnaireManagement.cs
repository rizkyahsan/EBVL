using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M019RefineQuestionnaireManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "EBVL",
                table: "QuestionnaireSections",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "AnswerRule",
                schema: "EBVL",
                table: "QuestionnaireQuestions",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Optional");

            migrationBuilder.AddColumn<string>(
                name: "CompanyType",
                schema: "EBVL",
                table: "QuestionnaireQuestions",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "EBVL",
                table: "QuestionnaireQuestions",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.Sql("UPDATE [EBVL].[QuestionnaireQuestions] SET [AnswerRule] = 'Mandatory' WHERE [IsRequired] = 1;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "EBVL",
                table: "QuestionnaireSections");

            migrationBuilder.DropColumn(
                name: "AnswerRule",
                schema: "EBVL",
                table: "QuestionnaireQuestions");

            migrationBuilder.DropColumn(
                name: "CompanyType",
                schema: "EBVL",
                table: "QuestionnaireQuestions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "EBVL",
                table: "QuestionnaireQuestions");
        }
    }
}
