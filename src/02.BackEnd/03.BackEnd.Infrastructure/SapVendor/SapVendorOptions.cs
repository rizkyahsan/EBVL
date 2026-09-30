namespace EBVL.BackEnd.Infrastructure.SapVendor;

public sealed record SapVendorOptions
{
    public const string SectionKey = "SapVendor";

    public bool Enabled { get; init; }
    public string EndpointUrl { get; init; } = string.Empty;
    public string SoapAction { get; init; } = "si_osGetEquiDet";
    public string RequestNamespace { get; init; } = "urn:pertamina:getpmdata";
    public string DefaultClient { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
