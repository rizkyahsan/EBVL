using System.ComponentModel.DataAnnotations;

namespace EBVL.Shared.Dto.Modules.Main.VendorRegistrations.SapVendor;

public record SapVendorRequest
{
    [Required(ErrorMessage = "Nomor SAP Vendor wajib diisi.")]
    public string SapVendorNumber { get; set; } = string.Empty;
}

public sealed class SapVendorRequestValidator : AbstractValidatorBase<SapVendorRequest>
{
    public SapVendorRequestValidator()
    {
        _ = RuleFor(x => x.SapVendorNumber)
            .NotEmpty()
            .MaximumLength(VendorRegistrationsMaximumLengthFor.SapVendorNumber);
    }
}
