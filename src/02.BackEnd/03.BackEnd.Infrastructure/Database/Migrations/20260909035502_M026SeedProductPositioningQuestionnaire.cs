using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M026SeedProductPositioningQuestionnaire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DECLARE @QuestionnaireId uniqueidentifier = (SELECT TOP (1) [Id] FROM [EBVL].[Questionnaires] WHERE [Code] = 'VENDOR_REGISTRATION' AND [IsDeleted] = 0);
                IF @QuestionnaireId IS NOT NULL
                BEGIN
                    DECLARE @SectionId uniqueidentifier = (SELECT TOP (1) [Id] FROM [EBVL].[QuestionnaireSections] WHERE [QuestionnaireId] = @QuestionnaireId AND [Code] = 'PRODUCT_POSITIONING');
                    IF @SectionId IS NULL
                    BEGIN
                        SET @SectionId = NEWID();
                        INSERT INTO [EBVL].[QuestionnaireSections] ([Id], [QuestionnaireId], [Code], [Title], [Order], [CompanyType], [IsActive], [IsDeleted], [Created], [CreatedBy])
                        VALUES (@SectionId, @QuestionnaireId, 'PRODUCT_POSITIONING', 'Product Positioning & Technical Support', 6, 'AuthorizedAgent', 1, 0, SYSDATETIMEOFFSET(), 'system@pertamina.com');
                    END
                    ELSE
                        UPDATE [EBVL].[QuestionnaireSections] SET [Title] = 'Product Positioning & Technical Support', [Order] = 6, [CompanyType] = 'AuthorizedAgent', [IsActive] = 1, [IsDeleted] = 0 WHERE [Id] = @SectionId;

                    MERGE [EBVL].[QuestionnaireQuestions] AS target
                    USING (VALUES
                        ('MARKET_SHARE_WORLD', 'Q045', 'Market Share (%) (In the world)', 'File', 1, 'Optional'),
                        ('BRAND_PRODUCT_COMPETITOR', 'Q048', 'Brand Product Competitor', 'File', 2, 'AddedValue'),
                        ('COUNTRY_ORIGIN_FACTORY_LOCATION', 'Q046', 'Country of Origin/Factory Location', 'File', 3, 'Mandatory'),
                        ('PRODUCT_TECHNOLOGY_LEADER_FOLLOWER', 'Q049', 'Product Technology Leader/Follower', 'ShortText', 4, 'Mandatory'),
                        ('PRODUCT_REGIONAL_SUPPLY', 'Q047', 'Product Regional Supply', 'File', 5, 'Mandatory'),
                        ('AFTER_SALES_SERVICE_OFFICE_VENDOR', 'Q050', 'After sales service office/vendor', 'ShortText', 6, 'AddedValue')
                    ) AS source ([Code], [LegacyCode], [Label], [Type], [Order], [AnswerRule])
                    ON target.[QuestionnaireSectionId] = @SectionId AND (target.[Code] = source.[Code] OR target.[Code] = source.[LegacyCode] OR target.[Label] = source.[Label])
                    WHEN MATCHED THEN UPDATE SET
                        target.[Code] = source.[Code], target.[Label] = source.[Label], target.[Type] = source.[Type], target.[Order] = source.[Order],
                        target.[IsRequired] = CASE WHEN source.[AnswerRule] = 'Mandatory' THEN 1 ELSE 0 END,
                        target.[IsVisible] = 1, target.[IsDeleted] = 0, target.[AnswerRule] = source.[AnswerRule], target.[CompanyType] = NULL, target.[IsActive] = 1
                    WHEN NOT MATCHED THEN INSERT
                        ([Id], [QuestionnaireSectionId], [Code], [Label], [Type], [Order], [IsRequired], [IsVisible], [IsDeleted], [Created], [CreatedBy], [AnswerRule], [CompanyType], [IsActive])
                    VALUES
                        (NEWID(), @SectionId, source.[Code], source.[Label], source.[Type], source.[Order], CASE WHEN source.[AnswerRule] = 'Mandatory' THEN 1 ELSE 0 END, 1, 0, SYSDATETIMEOFFSET(), 'system@pertamina.com', source.[AnswerRule], NULL, 1);
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
