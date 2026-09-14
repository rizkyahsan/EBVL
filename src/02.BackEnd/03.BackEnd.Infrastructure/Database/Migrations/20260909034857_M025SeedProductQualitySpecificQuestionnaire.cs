using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M025SeedProductQualitySpecificQuestionnaire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DECLARE @QuestionnaireId uniqueidentifier = (SELECT TOP (1) [Id] FROM [EBVL].[Questionnaires] WHERE [Code] = 'VENDOR_REGISTRATION' AND [IsDeleted] = 0);
                IF @QuestionnaireId IS NOT NULL
                BEGIN
                    DECLARE @SectionId uniqueidentifier = (SELECT TOP (1) [Id] FROM [EBVL].[QuestionnaireSections] WHERE [QuestionnaireId] = @QuestionnaireId AND [Code] = 'QUALITY_SPECIFIC');
                    IF @SectionId IS NULL
                    BEGIN
                        SET @SectionId = NEWID();
                        INSERT INTO [EBVL].[QuestionnaireSections] ([Id], [QuestionnaireId], [Code], [Title], [Order], [CompanyType], [IsActive], [IsDeleted], [Created], [CreatedBy])
                        VALUES (@SectionId, @QuestionnaireId, 'QUALITY_SPECIFIC', 'Product Quality - Spesific', 5, 'SoleDistributorAgent', 1, 0, SYSDATETIMEOFFSET(), 'system@pertamina.com');
                    END
                    ELSE
                        UPDATE [EBVL].[QuestionnaireSections] SET [Title] = 'Product Quality - Spesific', [Order] = 5, [CompanyType] = 'SoleDistributorAgent', [IsActive] = 1, [IsDeleted] = 0 WHERE [Id] = @SectionId;

                    MERGE [EBVL].[QuestionnaireQuestions] AS target
                    USING (VALUES
                        ('LATEST_PRODUCT_CATALOGUE', 'Q033', 'Latest Product Catalogue', 1, 'Mandatory'),
                        ('SAMPLE_FAT_REPORT', 'Q039', 'Sample of FAT Report', 2, 'AddedValue'),
                        ('LATEST_PRODUCT_EXPERIENCE_LIST', 'Q034', 'Latest Product Experience List', 3, 'Mandatory'),
                        ('SAMPLE_CONFORMITY_CERTIFICATE', 'Q040', 'Sample Conformity Certificate', 4, 'AddedValue'),
                        ('MANUFACTURING_TYPE_FULLY', 'Q035', 'Manufacturing Type - Fully (Design & Manufacturer) Packager/ Integrator', 5, 'Mandatory'),
                        ('SAMPLE_SITE_ACCEPTANCE_TEST_REPORT', 'Q041', 'Sample of Site Acceptance Test (SAT) Report', 6, 'Optional'),
                        ('INTERNATIONAL_PRODUCT_MFG_STANDARD_CERT', 'Q036', 'International Product Manufacturing Standard Certificate', 7, 'Mandatory'),
                        ('QUALITY_GUARANTEE_LETTER_HQ', 'Q042', 'Quality Guarantee Letter from HQ', 8, 'Mandatory'),
                        ('QUALITY_MANAGEMENT_SYSTEM', 'Q037', 'Quality Management System', 9, 'Mandatory'),
                        ('PRODUCT_OBSOLESCENCE_LETTER', 'Q043', 'Product Obsolescence Letter', 10, 'Optional'),
                        ('SAMPLE_INSPECTION_TEST_PLAN', 'Q038', 'Sample of Inspection Test Plan', 11, 'Mandatory'),
                        ('SAMPLE_TASA_PROGRAM', 'Q044', 'Sample of TASA (Technical Assistance Services Agreement) Program', 12, 'Optional')
                    ) AS source ([Code], [LegacyCode], [Label], [Order], [AnswerRule])
                    ON target.[QuestionnaireSectionId] = @SectionId AND (target.[Code] = source.[Code] OR target.[Code] = source.[LegacyCode] OR target.[Label] = source.[Label])
                    WHEN MATCHED THEN UPDATE SET
                        target.[Code] = source.[Code], target.[Label] = source.[Label], target.[Type] = 'File', target.[Order] = source.[Order],
                        target.[IsRequired] = CASE WHEN source.[AnswerRule] = 'Mandatory' THEN 1 ELSE 0 END,
                        target.[IsVisible] = 1, target.[IsDeleted] = 0, target.[AnswerRule] = source.[AnswerRule], target.[CompanyType] = NULL, target.[IsActive] = 1
                    WHEN NOT MATCHED THEN INSERT
                        ([Id], [QuestionnaireSectionId], [Code], [Label], [Type], [Order], [IsRequired], [IsVisible], [IsDeleted], [Created], [CreatedBy], [AnswerRule], [CompanyType], [IsActive])
                    VALUES
                        (NEWID(), @SectionId, source.[Code], source.[Label], 'File', source.[Order], CASE WHEN source.[AnswerRule] = 'Mandatory' THEN 1 ELSE 0 END, 1, 0, SYSDATETIMEOFFSET(), 'system@pertamina.com', source.[AnswerRule], NULL, 1);
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
