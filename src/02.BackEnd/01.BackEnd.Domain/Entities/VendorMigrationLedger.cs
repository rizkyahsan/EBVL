namespace EBVL.BackEnd.Domain.Entities;

public sealed class VendorMigrationRun
{
    public required Guid Id { get; set; }
    public required string SourceDatabase { get; set; }
    public required DateTimeOffset Started { get; set; }
    public DateTimeOffset? Completed { get; set; }
    public required string Status { get; set; }
    public int Imported { get; set; }
    public int Quarantined { get; set; }
}

public sealed class VendorMigrationRow
{
    public required Guid Id { get; set; }
    public required Guid RunId { get; set; }
    public required string SourceTable { get; set; }
    public required Guid SourceId { get; set; }
    public required string Outcome { get; set; }
    public string? Reason { get; set; }
    public DateTimeOffset Processed { get; set; }
}

public sealed class VendorMigrationCrosswalk
{
    public required Guid Id { get; set; }
    public required string SourceTable { get; set; }
    public required Guid SourceId { get; set; }
    public required string TargetTable { get; set; }
    public required Guid TargetId { get; set; }
    public required Guid RunId { get; set; }
    public DateTimeOffset Created { get; set; }
}

public sealed class VendorMigrationQuarantine
{
    public required Guid Id { get; set; }
    public required Guid RunId { get; set; }
    public required string SourceTable { get; set; }
    public required Guid SourceId { get; set; }
    public required string Reason { get; set; }
    public string? Detail { get; set; }
    public DateTimeOffset Created { get; set; }
}
