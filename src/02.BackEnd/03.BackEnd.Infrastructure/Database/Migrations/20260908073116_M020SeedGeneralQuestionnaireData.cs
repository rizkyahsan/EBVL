using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M020SeedGeneralQuestionnaireData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DECLARE @SectionId uniqueidentifier = (
                    SELECT TOP (1) s.[Id]
                    FROM [EBVL].[QuestionnaireSections] s
                    INNER JOIN [EBVL].[QuestionnaireVersions] v ON v.[Id] = s.[QuestionnaireVersionId]
                    INNER JOIN [EBVL].[Questionnaires] q ON q.[Id] = v.[QuestionnaireId]
                    WHERE q.[Code] = 'VENDOR_REGISTRATION' AND s.[Code] = 'GENERAL'
                      AND q.[IsDeleted] = 0 AND v.[IsDeleted] = 0 AND s.[IsDeleted] = 0
                    ORDER BY CASE WHEN v.[Status] = 'Draft' THEN 0 ELSE 1 END, v.[Version] DESC
                );

                IF @SectionId IS NOT NULL
                BEGIN
                    MERGE [EBVL].[QuestionnaireQuestions] AS target
                    USING (VALUES
                        ('BRAND_NAME', 'Q001', 'Name of Brand', 'Brand Name', 'ShortText', 1, 'Mandatory', NULL),
                        ('PRODUCT', 'Q002', 'Name of Product', 'Product', 'ShortText', 2, 'Mandatory', NULL),
                        ('VENDOR_COMPANY_NAME', 'Q003', 'Name of Vendor / Company', 'Vendor / Company Name', 'ShortText', 3, 'Mandatory', NULL),
                        ('COMPANY_ESTABLISHMENT', 'Q004', 'Company Establishment', 'Company Establishment', 'ShortText', 4, 'Mandatory', NULL),
                        ('HEADQUARTER_INFORMATION', 'Q006', 'Headquarter Address', 'Headquarter information', 'Address', 5, 'Mandatory', NULL),
                        ('INITIAL_PRODUCTION', 'Q005', 'Initial Production', 'Initial Production', 'ShortText', 6, 'Mandatory', NULL),
                        ('VENDOR_COMPANY_PROFILE', 'Q010', 'Vendor Company Profile', 'Vendor Company Profile', 'File', 7, 'Mandatory', 'Manufacture'),
                        ('SOLE_AGENT_COMPANY_PROFILE', 'Q012', 'Sole Agent Company Profile', 'Sole Agent Company Profile', 'File', 8, 'Optional', 'SoleDistributorAgent'),
                        ('LATEST_COMPANY_ANNUAL_REPORT', 'Q013', 'The Latest Company Annual Report', 'The Latest Company Annual Report', 'File', 9, 'AddedValue', NULL),
                        ('MANUFACTURING_INFORMATION', 'Q007', 'Manufacturing Address', 'Manufacturing information', 'Address', 10, 'Mandatory', 'Manufacture'),
                        ('REPRESENTATIVE_OFFICE_INFORMATION', 'Q008', 'Rep. Office Address', 'Representative Office information', 'Address', 11, 'Optional', 'AuthorizedAgent'),
                        ('SOLE_AGENT_OFFICE_INFORMATION', 'Q009', 'Sole Agent Address', 'Sole Agent Office information', 'Address', 12, 'Optional', 'SoleDistributorAgent')
                    ) AS source ([Code], [LegacyCode], [LegacyLabel], [Label], [Type], [Order], [AnswerRule], [CompanyType])
                    ON target.[QuestionnaireSectionId] = @SectionId
                       AND (target.[Code] = source.[Code] OR target.[Code] = source.[LegacyCode] OR target.[Label] = source.[LegacyLabel])
                    WHEN MATCHED THEN UPDATE SET
                        target.[Code] = source.[Code], target.[Label] = source.[Label], target.[Type] = source.[Type],
                        target.[Order] = source.[Order], target.[IsRequired] = CASE WHEN source.[AnswerRule] = 'Mandatory' THEN 1 ELSE 0 END,
                        target.[IsVisible] = 1, target.[IsDeleted] = 0, target.[AnswerRule] = source.[AnswerRule],
                        target.[CompanyType] = source.[CompanyType], target.[IsActive] = 1
                    WHEN NOT MATCHED THEN INSERT
                        ([Id], [QuestionnaireSectionId], [Code], [Label], [Type], [Order], [IsRequired], [IsVisible], [IsDeleted], [Created], [CreatedBy], [AnswerRule], [CompanyType], [IsActive])
                    VALUES
                        (NEWID(), @SectionId, source.[Code], source.[Label], source.[Type], source.[Order],
                         CASE WHEN source.[AnswerRule] = 'Mandatory' THEN 1 ELSE 0 END, 1, 0, SYSDATETIMEOFFSET(), 'system@pertamina.com', source.[AnswerRule], source.[CompanyType], 1);

                    UPDATE [EBVL].[QuestionnaireQuestions]
                    SET [IsDeleted] = 1
                    WHERE [QuestionnaireSectionId] = @SectionId AND [Code] = 'Q011';
                END;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Intentionally retained because questionnaire data may be edited after deployment.
        }
    }
}
