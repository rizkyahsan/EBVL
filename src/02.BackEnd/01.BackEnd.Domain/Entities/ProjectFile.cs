namespace EBVL.BackEnd.Domain.Entities;

public sealed class ProjectFile : ModifiableEntity
{
    public required Guid ProjectId { get; set; }

    public required Guid FileStorageId { get; set; }

    public Project Project { get; set; } = default!;
}
