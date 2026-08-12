namespace EBVL.BackEnd.Domain.Entities;

public sealed class VendorRegistration : ModifiableEntity
{
    public required string SapVendorNumber { get; set; }
    public required string CompanyName { get; set; }

    [Encrypted]
    public required string CompanyEmail { get; set; }

    [Encrypted]
    public required string PicEmail { get; set; }

    [Encrypted]
    public required string CompanyPhone { get; set; }

    [Encrypted]
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
    public required VendorRegistrationStatus Status { get; set; }
    public VendorAccount? Account { get; set; }
}
