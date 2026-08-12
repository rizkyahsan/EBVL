namespace EBVL.Shared.Dto.Modules.Administration.VendorRegistrations.AuthenticateVendor;

public sealed record AuthenticateVendorResponse
{
    public required Guid VendorAccountId { get; init; }
    public required Guid VendorRegistrationId { get; init; }
    public required string EmailAddress { get; init; }
    public required string CompanyName { get; init; }
    public required string SapVendorNumber { get; init; }
    public required VendorRegistrationStatus Status { get; init; }
}
