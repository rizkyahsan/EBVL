namespace EBVL.Shared.Dto.Modules.Main.VendorMonitoring.GetVendorRegistrations;

public sealed record VendorRegistrationItem
{
    public required Guid Id { get; init; }
    public required int Number { get; init; }
    public required string VendorName { get; init; }
    public required string SubmittedBy { get; init; }
    public required string Brand { get; init; }
    public required string ServiceType { get; init; }
    public required string Status { get; init; }
}
