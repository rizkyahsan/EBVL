namespace EBVL.BackEnd.Domain.Entities;

public sealed class Document : FileEntity
{
    public required string Description { get; set; }
}
