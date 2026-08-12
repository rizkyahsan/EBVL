namespace EBVL.Shared.Dto.Modules.Administration.VendorRegistrations.RegisterVendor;

public record RegisterVendorRequest
{
    public required string SapVendorNumber { get; set; }
    public required string CompanyName { get; set; }
    public required string CompanyEmail { get; set; }
    public required string PicEmail { get; set; }
    public required string CompanyPhone { get; set; }
    public required string PicPhone { get; set; }
    public string? Website { get; set; }
    public required string CompanyService { get; set; }
    public required string FactoryCountry { get; set; }
    public required string FactoryAddress { get; set; }
    public required string BrandRepresentative { get; set; }
    public required string CompanyStatus { get; set; }
    public required bool HasRepresentativeInIndonesia { get; set; }
    public string? IndonesiaRepresentativeName { get; set; }
    public required string BrandRegistrationLetterFileName { get; set; }
    public required string CompanyProfileFileName { get; set; }
    public required string ProductCatalogFileName { get; set; }
    public required string ProjectExperienceFileName { get; set; }
    public required string TaxCardFileName { get; set; }
    public required string MainCertificateFileName { get; set; }
    public required string Password { get; set; }
    public required string PasswordConfirmation { get; set; }
}

public sealed class RegisterVendorRequestValidator : AbstractValidatorBase<RegisterVendorRequest>
{
    public RegisterVendorRequestValidator()
    {
        _ = RuleFor(x => x.SapVendorNumber).NotEmpty().MaximumLength(50);
        _ = RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(250);
        _ = RuleFor(x => x.CompanyEmail).NotEmpty().EmailAddress().MaximumLength(320);
        _ = RuleFor(x => x.PicEmail).NotEmpty().EmailAddress().MaximumLength(320);
        _ = RuleFor(x => x.CompanyPhone).NotEmpty().MaximumLength(30);
        _ = RuleFor(x => x.PicPhone).NotEmpty().MaximumLength(30);
        _ = RuleFor(x => x.Website).MaximumLength(2048);
        _ = RuleFor(x => x.CompanyService).NotEmpty().MaximumLength(100);
        _ = RuleFor(x => x.FactoryCountry).NotEmpty().MaximumLength(100);
        _ = RuleFor(x => x.FactoryAddress).NotEmpty().MaximumLength(500);
        _ = RuleFor(x => x.BrandRepresentative).NotEmpty().MaximumLength(250);
        _ = RuleFor(x => x.CompanyStatus).NotEmpty().MaximumLength(100);
        _ = RuleFor(x => x.IndonesiaRepresentativeName).MaximumLength(250);
        _ = RuleFor(x => x.BrandRegistrationLetterFileName).NotEmpty().MaximumLength(200);
        _ = RuleFor(x => x.CompanyProfileFileName).NotEmpty().MaximumLength(200);
        _ = RuleFor(x => x.ProductCatalogFileName).NotEmpty().MaximumLength(200);
        _ = RuleFor(x => x.ProjectExperienceFileName).NotEmpty().MaximumLength(200);
        _ = RuleFor(x => x.TaxCardFileName).NotEmpty().MaximumLength(200);
        _ = RuleFor(x => x.MainCertificateFileName).NotEmpty().MaximumLength(200);
        _ = RuleFor(x => x.Password).NotEmpty().MinimumLength(12).MaximumLength(128)
            .Matches("[A-Z]").Matches("[a-z]").Matches("[0-9]");
        _ = RuleFor(x => x.PasswordConfirmation).Equal(x => x.Password);
    }
}
