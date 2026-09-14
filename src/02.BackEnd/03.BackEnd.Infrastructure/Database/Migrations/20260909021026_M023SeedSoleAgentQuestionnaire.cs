using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M023SeedSoleAgentQuestionnaire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DECLARE @QuestionnaireId uniqueidentifier = (
                    SELECT TOP (1) [Id] FROM [EBVL].[Questionnaires]
                    WHERE [Code] = 'VENDOR_REGISTRATION' AND [IsDeleted] = 0
                );

                IF @QuestionnaireId IS NOT NULL
                BEGIN
                    DECLARE @SectionId uniqueidentifier = (
                        SELECT TOP (1) [Id] FROM [EBVL].[QuestionnaireSections]
                        WHERE [QuestionnaireId] = @QuestionnaireId AND [Code] = 'SOLE_AGENT'
                    );

                    IF @SectionId IS NULL
                    BEGIN
                        SET @SectionId = NEWID();
                        INSERT INTO [EBVL].[QuestionnaireSections]
                            ([Id], [QuestionnaireId], [Code], [Title], [Order], [CompanyType], [IsActive], [IsDeleted], [Created], [CreatedBy])
                        VALUES
                            (@SectionId, @QuestionnaireId, 'SOLE_AGENT', 'Sole Agent',
                             (SELECT ISNULL(MAX([Order]), 0) + 1 FROM [EBVL].[QuestionnaireSections] WHERE [QuestionnaireId] = @QuestionnaireId AND [IsDeleted] = 0),
                             'SoleDistributorAgent', 1, 0, SYSDATETIMEOFFSET(), 'system@pertamina.com');
                    END
                    ELSE
                    BEGIN
                        UPDATE [EBVL].[QuestionnaireSections]
                        SET [Title] = 'Sole Agent', [CompanyType] = 'SoleDistributorAgent', [IsActive] = 1, [IsDeleted] = 0
                        WHERE [Id] = @SectionId;
                    END;

                    MERGE [EBVL].[QuestionnaireQuestions] AS target
                    USING (VALUES
                        ('SOLE_AGENT_BUSINESS_NAME_CARD_DIRECTOR', 'Q020', 'Business Card Name (Director)', 'Business Name Card (Director)', 1),
                        ('SKUP_MIGAS_ESDM', 'Q024', 'SKUP (Surat Kemampuan Usaha Penunjang) Migas/ESDM (Base on National Regulation)', 'SKUP (Surat Kemampuan Usaha Penunjang) Migas/ESDM (Base on National Regulation)', 2),
                        ('COMPANY_BUSINESS_LICENSES_SIUP', 'Q021', 'Company Business Licenses (SIUP)', 'Company Business Licenses (SIUP)', 3),
                        ('SKT_PERTAMINA', 'Q025', 'SKT (Surat Keterangan Terdaftar) Pertamina', 'SKT (Surat Keterangan Terdaftar) Pertamina', 4),
                        ('STP_TRADE_MINISTRY', 'Q022', 'STP (Surat Tanda Pendaftaran) Kemendag/ Trade Ministry', 'STP (Surat Tanda Pendaftaran) Kemendag/ Trade Ministry', 5),
                        ('CSMS_CERTIFICATE_PERTAMINA', 'Q026', 'CSMS (Contractor Safety Management System) Certificate Pertamina (Only for SA Provide service for product)', 'CSMS (Contractor Safety Management System) Certificate Pertamina (Only for SA Provide service for product)', 6),
                        ('AGENCY_AGREEMENT_FROM_VENDOR', 'Q023', 'Agency Agreement from Vendor', 'Agency Agreement from Vendor', 7)
                    ) AS source ([Code], [LegacyCode], [LegacyLabel], [Label], [Order])
                    ON target.[QuestionnaireSectionId] = @SectionId
                       AND (target.[Code] = source.[Code] OR target.[Code] = source.[LegacyCode] OR target.[Label] = source.[LegacyLabel])
                    WHEN MATCHED THEN UPDATE SET
                        target.[Code] = source.[Code], target.[Label] = source.[Label], target.[Type] = 'File',
                        target.[Order] = source.[Order], target.[IsRequired] = 1, target.[IsVisible] = 1,
                        target.[IsDeleted] = 0, target.[AnswerRule] = 'Mandatory',
                        target.[CompanyType] = 'SoleDistributorAgent', target.[IsActive] = 1
                    WHEN NOT MATCHED THEN INSERT
                        ([Id], [QuestionnaireSectionId], [Code], [Label], [Type], [Order], [IsRequired], [IsVisible], [IsDeleted], [Created], [CreatedBy], [AnswerRule], [CompanyType], [IsActive])
                    VALUES
                        (NEWID(), @SectionId, source.[Code], source.[Label], 'File', source.[Order], 1, 1, 0,
                         SYSDATETIMEOFFSET(), 'system@pertamina.com', 'Mandatory', 'SoleDistributorAgent', 1);
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
