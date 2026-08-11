namespace EBVL.BackEnd.Domain.Entities;

public sealed class Configuration : ModifiableEntity
{
    public required string Key { get; set; }
    public required string Value { get; set; }
}
