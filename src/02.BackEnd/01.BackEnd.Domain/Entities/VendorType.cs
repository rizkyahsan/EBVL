namespace EBVL.BackEnd.Domain.Entities;

public sealed class VendorType : ModifiableEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public ICollection<Vendor> Vendors { get; set; } = [];
}
