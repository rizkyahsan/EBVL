namespace EBVL.BackEnd.Services.SapVendor;

public interface ISapVendorService
{
    public Task<SapVendorLookupResult> LookupAsync(string sapVendorNumber, CancellationToken cancellationToken = default);
}

public sealed record SapVendorLookupResult(SapVendorLookupStatus Status, string? SapVendorNumber, string? Message = null);

public enum SapVendorLookupStatus
{
    Found,
    NotFound,
    Unavailable,
    Disabled
}
