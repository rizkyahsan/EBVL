using EBVL.Shared.Statics.VendorRegistrations;
using EBVL.Shared.Enums;

namespace EBVL.BackEnd.Infrastructure.Database.Seeders;

public sealed class QuestionnaireSeeder(IDatabaseService db)
{
    private static readonly Guid _questionnaireId = Guid.Parse("61000000-0000-0000-0000-000000000001");
    private static readonly Guid _versionId = Guid.Parse("61000000-0000-0000-0000-000000000002");
    private static Guid Id(int kind, int number)
    {
        return Guid.Parse($"61000000-0000-0000-{kind:0000}-{number:000000000000}");
    }

    public async Task SeedQuestionnaires()
    {
        if (await db.Questionnaires.AnyAsync(x => x.Code == "VENDOR_REGISTRATION"))
        {
            return;
        }

        var questionnaire = new Questionnaire { Id = _questionnaireId, Code = "VENDOR_REGISTRATION", BusinessProcess = "Vendor Registration", PublishedVersionId = _versionId };
        var version = new QuestionnaireVersion { Id = _versionId, QuestionnaireId = _questionnaireId, Version = 1, Status = QuestionnaireVersionStatus.Published, IsActive = true, PublishedAt = DateTimeOffset.Parse("2026-09-03T00:00:00Z") };
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
            var section = new QuestionnaireSection { Id = Id(1, s + 1), QuestionnaireVersionId = _versionId, Code = code, Title = title, Order = s + 1, CompanyType = companyType };
            for (var q = 0; q < questionOrder.Length; q++)
            {
                var source = QuestionnaireFor.All.Single(x => x.Number == questionOrder[q]);
                section.Questions.Add(new QuestionnaireQuestion { Id = Id(2, source.Number), Code = $"Q{source.Number:000}", Label = source.Label, Hint = source.Hint, Placeholder = source.Placeholder, Type = source.IsFileUpload ? QuestionnaireQuestionType.File : source.Number is 6 or 7 or 8 or 9 ? QuestionnaireQuestionType.Address : QuestionnaireQuestionType.ShortText, Order = q + 1, IsRequired = source.IsRequired, IsVisible = true });
            }

            version.Sections.Add(section);
        }

        questionnaire.Versions.Add(version);
        _ = await db.Questionnaires.AddAsync(questionnaire);
        _ = await db.SaveAsync(nameof(SeedQuestionnaires));
    }
}
