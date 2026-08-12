namespace EBVL.Shared.Dto.Modules.Administration.VendorRegistrations.RegisterVendor;

public sealed record RegisterVendorResponse
{
    public required Guid VendorRegistrationId { get; init; }
    public required Guid VendorId { get; init; }
    public required string CompanyName { get; init; }
    public required VendorRegistrationStatus Status { get; init; }
    public required int DocumentCount { get; init; }
    public required Guid CorrelationId { get; init; }
}
