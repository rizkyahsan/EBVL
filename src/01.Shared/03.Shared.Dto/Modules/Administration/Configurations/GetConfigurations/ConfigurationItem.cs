namespace EBVL.Shared.Dto.Modules.Administration.Configurations.GetConfigurations;

public sealed record ConfigurationItem
{
    public required Guid Id { get; init; }
    public required DateTimeOffset Created { get; init; }
    public required string CreatedBy { get; init; }
    public required DateTimeOffset? Modified { get; init; }
    public required string? ModifiedBy { get; init; }

    public required string Key { get; init; }
    public required string Value { get; init; }
}
