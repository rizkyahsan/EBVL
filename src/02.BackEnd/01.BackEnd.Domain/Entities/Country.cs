namespace EBVL.BackEnd.Domain.Entities;

public sealed class Country : ModifiableEntity
{
    public required string Name { get; set; }
    public required string Code { get; set; }       // e.g. "SG"
    public required string PhoneCode { get; set; }     // e.g. "+65"
    public required string CurrencyCode { get; set; }  // e.g. "SGD"
    public string? Region { get; set; }                // e.g. "Asia"

    public ICollection<Lender> Lenders { get; set; } = new HashSet<Lender>();
}
