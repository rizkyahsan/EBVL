namespace EBVL.Shared.Dto.Modules.Administration.ApiCalls.GetApiCalls;

public sealed record ApiCallItem
{
    public required Guid Id { get; init; }
    public required DateTimeOffset Created { get; init; }

    public required string ServiceName { get; init; }
    public required string ServiceProvider { get; init; }
    public required string ServiceCategory { get; init; }
    public required string RequestMethod { get; init; }
    public required ushort ResponseStatusCode { get; init; }
}
