using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.DocumentEvidence;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.PreRegistration;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;

public partial class VendorRegistrationReview
{
    [Parameter]
    public required PreRegistrationRequest PreRegistration { get; init; }

    [Parameter]
    public required DocumentEvidenceRequest DocumentEvidence { get; init; }

    [Parameter]
    public required EventCallback<string> OnDownload { get; init; }

    [Parameter]
    public required EventCallback OnSendForVerification { get; init; }

    private string GetBrands()
    {
        return string.Join(", ", new[] { PreRegistration.BrandRepresentative }.Concat(PreRegistration.AdditionalBrands).Where(brand => !string.IsNullOrWhiteSpace(brand)));
    }

    private string? GetFileName(string key)
    {
        return DocumentEvidence.Documents.Single(document => document.Key == key).FileName;
    }
}
