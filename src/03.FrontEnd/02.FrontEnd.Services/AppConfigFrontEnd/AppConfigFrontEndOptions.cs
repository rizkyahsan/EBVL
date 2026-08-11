namespace EBVL.FrontEnd.Services.AppConfigFrontEnd;

public sealed record AppConfigFrontEndOptions
{
    public const string SectionKey = nameof(AppConfigFrontEnd);

    public required string AppNickName { get; init; }
    public required string AppFullName { get; init; }
    public required string PathBase { get; init; }
    public required string BackEndApiBaseUrl { get; init; }
}
