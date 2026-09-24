using EBVL.Shared.Enums;

namespace EBVL.Shared.Dto.Modules.Main.Vendor.RequestRegistrations.GetRequestRegistrations;

public sealed record RequestRegistrationItem
{
    public required Guid Id { get; init; }
    public required string VendorName { get; init; }
    public required VendorRegistrationStatus Status { get; init; }
    public required string StatusDisplay { get; init; }
    public required RequestRegistrationCategory Category { get; init; }
    public required DateTimeOffset SubmittedAt { get; init; }
    public required DateTimeOffset LastUpdatedAt { get; init; }
}
