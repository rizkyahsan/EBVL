namespace EBVL.Shared.Dto.Modules.Examples.Documents.GetDocuments;

public sealed record DocumentItem
{
    public required Guid Id { get; init; }
    public required DateTimeOffset Created { get; init; }
    public required string CreatedBy { get; init; }

    public required string FileName { get; init; }
    public required long FileSize { get; init; }
}
