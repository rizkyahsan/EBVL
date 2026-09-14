using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M024SeedProductQualityGeneralQuestionnaire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DECLARE @QuestionnaireId uniqueidentifier = (SELECT TOP (1) [Id] FROM [EBVL].[Questionnaires] WHERE [Code] = 'VENDOR_REGISTRATION' AND [IsDeleted] = 0);
                IF @QuestionnaireId IS NOT NULL
                BEGIN
                    DECLARE @SectionId uniqueidentifier = (SELECT TOP (1) [Id] FROM [EBVL].[QuestionnaireSections] WHERE [QuestionnaireId] = @QuestionnaireId AND [Code] = 'QUALITY_GENERAL');
                    IF @SectionId IS NULL
                    BEGIN
                        SET @SectionId = NEWID();
                        INSERT INTO [EBVL].[QuestionnaireSections] ([Id], [QuestionnaireId], [Code], [Title], [Order], [CompanyType], [IsActive], [IsDeleted], [Created], [CreatedBy])
                        VALUES (@SectionId, @QuestionnaireId, 'QUALITY_GENERAL', 'Product Quality - General',
                            (SELECT ISNULL(MAX([Order]), 0) + 1 FROM [EBVL].[QuestionnaireSections] WHERE [QuestionnaireId] = @QuestionnaireId AND [IsDeleted] = 0),
                            NULL, 1, 0, SYSDATETIMEOFFSET(), 'system@pertamina.com');
                    END
                    ELSE
                        UPDATE [EBVL].[QuestionnaireSections] SET [Title] = 'Product Quality - General', [CompanyType] = NULL, [IsActive] = 1, [IsDeleted] = 0 WHERE [Id] = @SectionId;

                    MERGE [EBVL].[QuestionnaireQuestions] AS target
                    USING (VALUES
                        ('BRAND_CERTIFICATE', 'Q027', 'Brand Certificate', 1),
                        ('SAMPLE_COMPONENT_MILL_CERTIFICATE', 'Q030', 'Sample Component Mill Certificate', 2),
                        ('PATENT_LICENSE_PRODUCT_DESIGN_CERTIFICATE', 'Q028', 'Patent/License Certificate and/or International Product Design Standard Certificate', 3),
                        ('USER_SATISFACTION_LETTER', 'Q031', 'User Satisfaction Letter (Testimony)', 4),
                        ('COMPONENT_SUPPLIER_EXPERT_LIST', 'Q029', 'Component Supplier/Expert List', 5),
                        ('LOCAL_CONTENT_CERTIFICATE_TKDN', 'Q032', 'Local Content Certificate (TKDN from Kemenperin)', 6)
                    ) AS source ([Code], [LegacyCode], [Label], [Order])
                    ON target.[QuestionnaireSectionId] = @SectionId AND (target.[Code] = source.[Code] OR target.[Code] = source.[LegacyCode] OR target.[Label] = source.[Label])
                    WHEN MATCHED THEN UPDATE SET
                        target.[Code] = source.[Code], target.[Label] = source.[Label], target.[Type] = 'File', target.[Order] = source.[Order],
                        target.[IsRequired] = 0, target.[IsVisible] = 1, target.[IsDeleted] = 0, target.[AnswerRule] = 'AddedValue', target.[CompanyType] = NULL, target.[IsActive] = 1
                    WHEN NOT MATCHED THEN INSERT
                        ([Id], [QuestionnaireSectionId], [Code], [Label], [Type], [Order], [IsRequired], [IsVisible], [IsDeleted], [Created], [CreatedBy], [AnswerRule], [CompanyType], [IsActive])
                    VALUES
                        (NEWID(), @SectionId, source.[Code], source.[Label], 'File', source.[Order], 0, 1, 0, SYSDATETIMEOFFSET(), 'system@pertamina.com', 'AddedValue', NULL, 1);
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
