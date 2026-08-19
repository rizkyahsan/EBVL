namespace EBVL.BackEnd.Domain.Entities;

public sealed class Status : ModifiableEntity
{
    public required string Table { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
}
