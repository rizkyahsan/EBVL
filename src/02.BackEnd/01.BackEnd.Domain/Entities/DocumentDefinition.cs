namespace EBVL.BackEnd.Domain.Entities;

public sealed class DocumentDefinition : ModifiableEntity
{
    public required string Code { get; set; }
    public required string BusinessProcess { get; set; }
    public required string Name { get; set; }
    public int Order { get; set; }
    public int MaxSizeMb { get; set; }
    public bool IsMandatory { get; set; }
    public bool IsActive { get; set; }
    public byte[] RowVersion { get; set; } = [];
}
