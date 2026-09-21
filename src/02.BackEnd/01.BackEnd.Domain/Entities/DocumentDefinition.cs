namespace EBVL.BackEnd.Domain.Entities;

public sealed class DocumentDefinition : ModifiableEntity
{
    public Guid DocumentRequirementSetId { get; set; }
    public required string Code { get; set; }
    public required string BusinessProcess { get; set; }
    public required string Name { get; set; }
    public int Order { get; set; }
    public int MaxSizeMb { get; set; }
    public bool IsMandatory { get; set; }
    public bool IsActive { get; set; }
    public byte[] RowVersion { get; set; } = [];
    public DocumentRequirementSet DocumentRequirementSet { get; set; } = default!;
}

public sealed class DocumentRequirementSet : ModifiableEntity
{
    public Guid DocumentRequirementSetSeriesId { get; set; }
    public required string BusinessProcess { get; set; }
    public int Version { get; set; } = 1;
    public QuestionnaireStatus Status { get; set; } = QuestionnaireStatus.Draft;
    public Guid? PreviousVersionId { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public string? PublishedBy { get; set; }
    public byte[] RowVersion { get; set; } = [];
    public DocumentRequirementSet? PreviousVersion { get; set; }
    public ICollection<DocumentDefinition> Requirements { get; set; } = new HashSet<DocumentDefinition>();
}
