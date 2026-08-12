namespace EBVL.Shared.Dto.Modules.Administration.VendorRegistrations.RegisterVendor;

public sealed record VendorRegistrationFileItem
{
    public required byte[] FileContent { get; init; }
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
}
