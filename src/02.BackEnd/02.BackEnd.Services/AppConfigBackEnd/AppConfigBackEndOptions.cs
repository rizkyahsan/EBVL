namespace EBVL.BackEnd.Services.AppConfigBackEnd;

public sealed record AppConfigBackEndOptions
{
    public const string SectionKey = nameof(AppConfigBackEnd);

    public required string AppNickName { get; init; }
    public required string AppFullName { get; init; }
    public required string PathBase { get; init; }
    public required string FrontEndBaseUrl { get; init; }
    public required bool IsDataSeedingEnabled { get; init; }
}
