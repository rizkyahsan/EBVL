namespace EBVL.BackEnd.Domain.Entities;

public sealed class ContactType : ModifiableEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public ICollection<VendorContact> Contacts { get; set; } = [];
}
