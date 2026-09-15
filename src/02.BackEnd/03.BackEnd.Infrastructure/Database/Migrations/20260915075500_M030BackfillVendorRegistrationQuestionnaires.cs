using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations;

public partial class M030BackfillVendorRegistrationQuestionnaires : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            UPDATE registrations
            SET registrations.QuestionnaireId = questionnaires.Id
            FROM [EBVL].[VendorRegistrations] registrations
            CROSS JOIN [EBVL].[Questionnaires] questionnaires
            WHERE registrations.QuestionnaireId IS NULL
              AND registrations.IsDeleted = 0
              AND questionnaires.Code = 'VENDOR_REGISTRATION'
              AND questionnaires.BusinessProcess = 'Vendor Registration'
              AND questionnaires.IsActive = 1
              AND questionnaires.IsDeleted = 0;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}
