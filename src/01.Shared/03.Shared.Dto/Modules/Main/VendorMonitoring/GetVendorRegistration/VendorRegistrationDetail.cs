namespace EBVL.Shared.Dto.Modules.Main.VendorMonitoring.GetVendorRegistration;

public sealed record VendorRegistrationDetail
{
    public required Guid Id { get; init; }
    public required string CompanyName { get; init; }
    public required string CompanyEmail { get; init; }
    public required string PicEmail { get; init; }
    public required string CompanyPhoneNumber { get; init; }
    public required string PicPhoneNumber { get; init; }
    public required string Website { get; init; }
    public required string CompanyService { get; init; }
    public required string FactoryAddress { get; init; }
    public required string BrandRepresentative { get; init; }
    public required string CompanyStatus { get; init; }
    public required string SapVendorNumber { get; init; }
    public required string TaxNumber { get; init; }
}
