using System.ComponentModel.DataAnnotations;
using EBVL.Shared.Enums;

namespace EBVL.Shared.Dto.Modules.Main.VendorRegistrations.PreRegistration;

public record PreRegistrationRequest
{
    public string SapVendorNumber { get; set; } = string.Empty;

    [Required]
    public string CompanyName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string CompanyEmail { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string PicEmail { get; set; } = string.Empty;

    [Required]
    public string CompanyPhoneNumber { get; set; } = string.Empty;

    [Required]
    public string PicPhoneNumber { get; set; } = string.Empty;

    public string Website { get; set; } = string.Empty;

    [Required]
    public VendorServiceType? CompanyService { get; set; }

    [Required]
    public string FactoryCountry { get; set; } = string.Empty;

    [Required]
    public string FactoryAddress { get; set; } = string.Empty;

    [Required]
    public string BrandRepresentative { get; set; } = string.Empty;

    public List<string> AdditionalBrands { get; set; } = [];

    [Required]
    public VendorCompanyStatusType? CompanyStatus { get; set; }

    [Required]
    public bool? IsRepresentativeInIndonesia { get; set; }

    public string RepresentativeName { get; set; } = string.Empty;
}

public sealed class PreRegistrationRequestValidator : AbstractValidatorBase<PreRegistrationRequest>
{
    public PreRegistrationRequestValidator()
    {
        _ = RuleFor(x => x.SapVendorNumber)
            .NotEmpty()
            .MaximumLength(VendorRegistrationsMaximumLengthFor.SapVendorNumber);

        _ = RuleFor(x => x.CompanyName)
            .NotEmpty()
            .MaximumLength(VendorRegistrationsMaximumLengthFor.CompanyName);

        _ = RuleFor(x => x.CompanyEmail)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(CommonMaximumLengthFor.EmailAddress);

        _ = RuleFor(x => x.PicEmail)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(CommonMaximumLengthFor.EmailAddress);

        _ = RuleFor(x => x.CompanyPhoneNumber)
            .NotEmpty()
            .MaximumLength(CommonMaximumLengthFor.PhoneNumber);

        _ = RuleFor(x => x.PicPhoneNumber)
            .NotEmpty()
            .MaximumLength(CommonMaximumLengthFor.PhoneNumber);

        _ = RuleFor(x => x.Website)
            .MaximumLength(CommonMaximumLengthFor.Url);

        _ = RuleFor(x => x.CompanyService).NotNull();
        _ = RuleFor(x => x.FactoryCountry).NotEmpty();
        _ = RuleFor(x => x.FactoryAddress)
            .NotEmpty()
            .MaximumLength(VendorRegistrationsMaximumLengthFor.FactoryAddress);
        _ = RuleFor(x => x.BrandRepresentative)
            .NotEmpty()
            .MaximumLength(VendorRegistrationsMaximumLengthFor.BrandName);
        _ = RuleForEach(x => x.AdditionalBrands)
            .NotEmpty()
            .MaximumLength(VendorRegistrationsMaximumLengthFor.BrandName);
        _ = RuleFor(x => x.CompanyStatus).NotNull();
        _ = RuleFor(x => x.IsRepresentativeInIndonesia).NotNull();
    }
}
