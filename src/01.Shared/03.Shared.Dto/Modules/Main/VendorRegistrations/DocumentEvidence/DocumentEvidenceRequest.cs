namespace EBVL.Shared.Dto.Modules.Main.VendorRegistrations.DocumentEvidence;

public record DocumentEvidenceRequest
{
    public string SapVendorNumber { get; set; } = string.Empty;
    public List<DocumentEvidenceItemRequest> Documents { get; set; } =
    [
        .. EBVL.Shared.Statics.VendorRegistrations.DocumentEvidenceFor.All.Select(document => new DocumentEvidenceItemRequest
        {
            Key = document.Key
        })
    ];
}

public sealed class DocumentEvidenceRequestValidator : AbstractValidatorBase<DocumentEvidenceRequest>
{
    public DocumentEvidenceRequestValidator()
    {
        _ = RuleFor(x => x.SapVendorNumber).NotEmpty();
        _ = RuleForEach(x => x.Documents)
            .Must(document => !string.IsNullOrWhiteSpace(document.FileName))
            .WithMessage("Seluruh dokumen evidence wajib diunggah.");
    }
}
