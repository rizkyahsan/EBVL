using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.DocumentEvidence;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.PreRegistration;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaire;
using EBVL.Shared.Statics.VendorRegistrations;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;

public partial class VendorRegistrationReview
{
    [Parameter]
    public required PreRegistrationRequest PreRegistration { get; init; }

    [Parameter]
    public required DocumentEvidenceRequest DocumentEvidence { get; init; }

    [Parameter]
    public required QuestionnaireRequest Questionnaire { get; init; }

    [Parameter]
    public required EventCallback<string> OnDownload { get; init; }

    [Parameter]
    public required EventCallback OnSendForVerification { get; init; }

    [Parameter]
    public required EventCallback OnPrevious { get; init; }

    private bool _isConfirmed;

    private string GetBrands()
    {
        return string.Join(", ", new[] { PreRegistration.BrandRepresentative }.Concat(PreRegistration.AdditionalBrands).Where(brand => !string.IsNullOrWhiteSpace(brand)));
    }

    private string? GetFileName(string key)
    {
        return DocumentEvidence.Documents.SingleOrDefault(document => document.Key == key)?.FileName;
    }

    private string GetAnswer(int questionNumber)
    {
        var answer = Questionnaire.Answers.SingleOrDefault(item => item.QuestionNumber == questionNumber);
        return answer?.FileName ?? answer?.Value ?? "-";
    }

    private static string FormatAddress(QuestionnaireAddressRequest address)
    {
        var values = new[]
        {
            address.Building,
            address.Street,
            address.Number,
            address.City,
            address.Country,
            address.Phone,
            address.Fax,
            address.Email,
            address.Website
        };

        return string.Join(", ", values.Where(value => !string.IsNullOrWhiteSpace(value)));
    }

    private static string GetSectionTitle(string section)
    {
        return section switch
        {
            QuestionnaireFor.GeneralInformation => "General",
            QuestionnaireFor.VendorRepresentativeOffice => "Vendor / RO (Representative Office)",
            QuestionnaireFor.SoleAgent => "Sole Agent",
            QuestionnaireFor.ProductQualityGeneral => "Product Quality - General",
            QuestionnaireFor.ProductQualitySpecific => "Product Quality - Spesific",
            _ => "Product Positioning & Technical Support"
        };
    }
}
