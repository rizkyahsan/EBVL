using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M033NormalizeQuestionnairePublishStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Questionnaires_OnePublishedPerSeries",
                schema: "EBVL",
                table: "Questionnaires");

            migrationBuilder.Sql("""
                UPDATE [EBVL].[Questionnaires]
                SET [Status] = 'Publish'
                WHERE [Status] = 'Published';
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaires_OnePublishPerSeries",
                schema: "EBVL",
                table: "Questionnaires",
                column: "QuestionnaireSeriesId",
                unique: true,
                filter: "[Status] = 'Publish' AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireSections_QuestionnaireId_Order",
                schema: "EBVL",
                table: "QuestionnaireSections",
                columns: new[] { "QuestionnaireId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireQuestions_QuestionnaireSectionId_Order",
                schema: "EBVL",
                table: "QuestionnaireQuestions",
                columns: new[] { "QuestionnaireSectionId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireOptions_QuestionnaireQuestionId_Order",
                schema: "EBVL",
                table: "QuestionnaireOptions",
                columns: new[] { "QuestionnaireQuestionId", "Order" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Questionnaires_OnePublishPerSeries",
                schema: "EBVL",
                table: "Questionnaires");

            migrationBuilder.Sql("""
                UPDATE [EBVL].[Questionnaires]
                SET [Status] = 'Published'
                WHERE [Status] = 'Publish';
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaires_OnePublishedPerSeries",
                schema: "EBVL",
                table: "Questionnaires",
                column: "QuestionnaireSeriesId",
                unique: true,
                filter: "[Status] = 'Published' AND [IsDeleted] = 0");

            migrationBuilder.DropIndex(
                name: "IX_QuestionnaireSections_QuestionnaireId_Order",
                schema: "EBVL",
                table: "QuestionnaireSections");

            migrationBuilder.DropIndex(
                name: "IX_QuestionnaireQuestions_QuestionnaireSectionId_Order",
                schema: "EBVL",
                table: "QuestionnaireQuestions");

            migrationBuilder.DropIndex(
                name: "IX_QuestionnaireOptions_QuestionnaireQuestionId_Order",
                schema: "EBVL",
                table: "QuestionnaireOptions");
        }
    }
}
