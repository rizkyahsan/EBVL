using EBVL.Shared.Statics.VendorRegistrations;
using EBVL.Shared.Enums;

namespace EBVL.BackEnd.Infrastructure.Database.Seeders;

public sealed class QuestionnaireSeeder(IDatabaseService db)
{
    private static readonly Guid _questionnaireId = Guid.Parse("61000000-0000-0000-0000-000000000001");
    private static Guid Id(int kind, int number)
    {
        return Guid.Parse($"61000000-0000-0000-{kind:0000}-{number:000000000000}");
    }

    public async Task SeedQuestionnaires()
    {
        var existing = await db.Questionnaires.Include(x => x.Sections).ThenInclude(x => x.Questions)
            .SingleOrDefaultAsync(x => x.Code == "VENDOR_REGISTRATION");
        if (existing is not null)
        {
            await EnsureGeneralQuestions(existing);
            return;
        }

        var questionnaire = new Questionnaire { Id = _questionnaireId, Code = "VENDOR_REGISTRATION", BusinessProcess = "Vendor Registration", IsActive = true };
        (string Title, string Code, VendorCompanyStatusType? CompanyType, int[] Order)[] definitions =
        {
            (QuestionnaireFor.GeneralInformation, "GENERAL", null, Enumerable.Range(1,13).ToArray()),
            (QuestionnaireFor.VendorRepresentativeOffice, "VENDOR_RO", VendorCompanyStatusType.AuthorizedAgent, new[] {14,17,15,18,16,19}),
            (QuestionnaireFor.SoleAgent, "SOLE_AGENT", VendorCompanyStatusType.SoleDistributorAgent, new[] {20,24,21,25,22,26,23}),
            (QuestionnaireFor.ProductQualityGeneral, "QUALITY_GENERAL", null, [27,30,28,31,29,32]),
            (QuestionnaireFor.ProductQualitySpecific, "QUALITY_SPECIFIC", VendorCompanyStatusType.SoleDistributorAgent, new[] {33,39,34,40,35,41,36,42,37,43,38,44}),
            (QuestionnaireFor.ProductPositioning, "PRODUCT_POSITIONING", VendorCompanyStatusType.AuthorizedAgent, new[] {45,48,46,49,47,50})
        };
        for (var s = 0; s < definitions.Length; s++)
        {
            var (title, code, companyType, questionOrder) = definitions[s];
            var section = new QuestionnaireSection { Id = Id(1, s + 1), QuestionnaireId = _questionnaireId, Code = code, Title = title, Order = s + 1, CompanyType = companyType, IsActive = true };
            for (var q = 0; q < questionOrder.Length; q++)
            {
                var source = QuestionnaireFor.All.Single(x => x.Number == questionOrder[q]);
                section.Questions.Add(new QuestionnaireQuestion { Id = Id(2, source.Number), Code = $"Q{source.Number:000}", Label = source.Label, Hint = source.Hint, Placeholder = source.Placeholder, Type = source.IsFileUpload ? QuestionnaireQuestionType.File : source.Number is 6 or 7 or 8 or 9 ? QuestionnaireQuestionType.Address : QuestionnaireQuestionType.ShortText, Order = q + 1, IsRequired = source.IsRequired, IsVisible = true, IsActive = true, AnswerRule = source.IsRequired ? QuestionnaireAnswerRule.Mandatory : QuestionnaireAnswerRule.Optional });
            }

            questionnaire.Sections.Add(section);
        }

        _ = await db.Questionnaires.AddAsync(questionnaire);
        _ = await db.SaveAsync(nameof(SeedQuestionnaires));
        await EnsureGeneralQuestions(questionnaire);
    }

    private async Task EnsureGeneralQuestions(Questionnaire questionnaire)
    {
        var section = questionnaire.Sections.SingleOrDefault(x => x.Code == "GENERAL");
        if (section is null)
        {
            section = new QuestionnaireSection { QuestionnaireId = questionnaire.Id, Code = "GENERAL", Title = "General", Order = 1, IsActive = true };
            questionnaire.Sections.Add(section);
        }

        (string Code, string LegacyCode, string Label, QuestionnaireQuestionType Type, QuestionnaireAnswerRule Rule, VendorCompanyStatusType? CompanyType)[] definitions =
        {
            ("BRAND_NAME", "Q001", "Brand Name", QuestionnaireQuestionType.ShortText, QuestionnaireAnswerRule.Mandatory, null),
            ("PRODUCT", "Q002", "Product", QuestionnaireQuestionType.ShortText, QuestionnaireAnswerRule.Mandatory, null),
            ("VENDOR_COMPANY_NAME", "Q003", "Vendor / Company Name", QuestionnaireQuestionType.ShortText, QuestionnaireAnswerRule.Mandatory, null),
            ("COMPANY_ESTABLISHMENT", "Q004", "Company Establishment", QuestionnaireQuestionType.ShortText, QuestionnaireAnswerRule.Mandatory, null),
            ("HEADQUARTER_INFORMATION", "Q006", "Headquarter information", QuestionnaireQuestionType.Address, QuestionnaireAnswerRule.Mandatory, null),
            ("INITIAL_PRODUCTION", "Q005", "Initial Production", QuestionnaireQuestionType.ShortText, QuestionnaireAnswerRule.Mandatory, null),
            ("VENDOR_COMPANY_PROFILE", "Q010", "Vendor Company Profile", QuestionnaireQuestionType.File, QuestionnaireAnswerRule.Mandatory, VendorCompanyStatusType.Manufacture),
            ("SOLE_AGENT_COMPANY_PROFILE", "Q012", "Sole Agent Company Profile", QuestionnaireQuestionType.File, QuestionnaireAnswerRule.Optional, VendorCompanyStatusType.SoleDistributorAgent),
            ("LATEST_COMPANY_ANNUAL_REPORT", "Q013", "The Latest Company Annual Report", QuestionnaireQuestionType.File, QuestionnaireAnswerRule.AddedValue, null),
            ("MANUFACTURING_INFORMATION", "Q007", "Manufacturing information", QuestionnaireQuestionType.Address, QuestionnaireAnswerRule.Mandatory, VendorCompanyStatusType.Manufacture),
            ("REPRESENTATIVE_OFFICE_INFORMATION", "Q008", "Representative Office information", QuestionnaireQuestionType.Address, QuestionnaireAnswerRule.Optional, VendorCompanyStatusType.AuthorizedAgent),
            ("SOLE_AGENT_OFFICE_INFORMATION", "Q009", "Sole Agent Office information", QuestionnaireQuestionType.Address, QuestionnaireAnswerRule.Optional, VendorCompanyStatusType.SoleDistributorAgent)
        };

        for (var index = 0; index < definitions.Length; index++)
        {
            var definition = definitions[index];
            var question = section.Questions.FirstOrDefault(x => x.Code == definition.Code || x.Code == definition.LegacyCode || x.Label == definition.Label || (definition.Code == "BRAND_NAME" && x.Label == "Name of Brand"));
            if (question is null)
            {
                question = new QuestionnaireQuestion { QuestionnaireSectionId = section.Id, Code = definition.Code, Label = definition.Label };
                section.Questions.Add(question);
            }

            question.Code = definition.Code;
            question.Label = definition.Label;
            question.Type = definition.Type;
            question.Order = index + 1;
            question.IsRequired = definition.Rule == QuestionnaireAnswerRule.Mandatory;
            question.IsVisible = true;
            question.IsActive = true;
            question.AnswerRule = definition.Rule;
            question.CompanyType = definition.CompanyType;
        }

        var obsoleteRepresentativeProfile = section.Questions.FirstOrDefault(x => x.Code == "Q011");
        _ = obsoleteRepresentativeProfile?.IsDeleted = true;

        _ = await db.SaveAsync(nameof(EnsureGeneralQuestions));
    }
}
