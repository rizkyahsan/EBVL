using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBVL.BackEnd.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class M022SeedVendorRepresentativeOfficeQuestionnaire : Migration
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
                        WHERE [QuestionnaireId] = @QuestionnaireId AND [Code] = 'VENDOR_RO'
                    );

                    IF @SectionId IS NULL
                    BEGIN
                        SET @SectionId = NEWID();
                        INSERT INTO [EBVL].[QuestionnaireSections]
                            ([Id], [QuestionnaireId], [Code], [Title], [Order], [CompanyType], [IsActive], [IsDeleted], [Created], [CreatedBy])
                        VALUES
                            (@SectionId, @QuestionnaireId, 'VENDOR_RO', 'Vendor / RO (Representative Office)',
                             (SELECT ISNULL(MAX([Order]), 0) + 1 FROM [EBVL].[QuestionnaireSections] WHERE [QuestionnaireId] = @QuestionnaireId AND [IsDeleted] = 0),
                             'AuthorizedAgent', 1, 0, SYSDATETIMEOFFSET(), 'system@pertamina.com');
                    END
                    ELSE
                    BEGIN
                        UPDATE [EBVL].[QuestionnaireSections]
                        SET [Title] = 'Vendor / RO (Representative Office)', [CompanyType] = 'AuthorizedAgent', [IsActive] = 1, [IsDeleted] = 0
                        WHERE [Id] = @SectionId;
                    END;

                    MERGE [EBVL].[QuestionnaireQuestions] AS target
                    USING (VALUES
                        ('BUSINESS_NAME_CARD_DIRECTOR', 'Q014', 'Business Card Name (Director)', 'Business Name Card (Director)', 1, 'Mandatory'),
                        ('CERTIFICATE_COMPANY_DOMICILE', 'Q017', 'Certificate of Company Domicile (COD/Surat Keterangan Domisili)', 'Certificate of Company Domicile (COD/Surat Keterangan Domisili)', 2, 'Mandatory'),
                        ('ID_CARD_PASSPORT_AUTHORIZED_SIGNATORIES', 'Q015', 'ID Card/Passport for authorized Signatories', 'ID Card/Passport for authorized Signatories', 3, 'AddedValue'),
                        ('TAX_IDENTIFICATION_NUMBER_COMPANY', 'Q018', 'Tax Identification Number (TIN/NPWP)-Company', 'Tax Identification Number (TIN/NPWP)-Company', 4, 'Mandatory'),
                        ('DEED_COMPANY_ESTABLISHMENT', 'Q016', 'Deed of Company Establishment (ACTA/ Akta Pendirian Perusahaan)', 'Deed of Company Establishment (ACTA/ Akta Pendirian Perusahaan)', 5, 'Mandatory'),
                        ('CERTIFICATE_COMPANY_REGISTRATION', 'Q019', 'Certificate of Company Registration (TDP)', 'Certificate of Company Registration (TDP)', 6, 'Mandatory')
                    ) AS source ([Code], [LegacyCode], [LegacyLabel], [Label], [Order], [AnswerRule])
                    ON target.[QuestionnaireSectionId] = @SectionId
                       AND (target.[Code] = source.[Code] OR target.[Code] = source.[LegacyCode] OR target.[Label] = source.[LegacyLabel])
                    WHEN MATCHED THEN UPDATE SET
                        target.[Code] = source.[Code], target.[Label] = source.[Label], target.[Type] = 'File',
                        target.[Order] = source.[Order], target.[IsRequired] = CASE WHEN source.[AnswerRule] = 'Mandatory' THEN 1 ELSE 0 END,
                        target.[IsVisible] = 1, target.[IsDeleted] = 0, target.[AnswerRule] = source.[AnswerRule],
                        target.[CompanyType] = 'AuthorizedAgent', target.[IsActive] = 1
                    WHEN NOT MATCHED THEN INSERT
                        ([Id], [QuestionnaireSectionId], [Code], [Label], [Type], [Order], [IsRequired], [IsVisible], [IsDeleted], [Created], [CreatedBy], [AnswerRule], [CompanyType], [IsActive])
                    VALUES
                        (NEWID(), @SectionId, source.[Code], source.[Label], 'File', source.[Order],
                         CASE WHEN source.[AnswerRule] = 'Mandatory' THEN 1 ELSE 0 END, 1, 0, SYSDATETIMEOFFSET(), 'system@pertamina.com', source.[AnswerRule], 'AuthorizedAgent', 1);
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
