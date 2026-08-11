namespace EBVL.BackEnd.Domain.Entities;

public sealed class PublicHoliday : ModifiableEntity
{
    public required string CountryCode { get; set; }
    public required DateOnly Date { get; set; }
    public required string Name { get; set; }
    public required string LocalName { get; set; }
}
