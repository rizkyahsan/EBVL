namespace EBVL.BackEnd.Domain.Entities;

public sealed class VendorContact : ModifiableEntity
{
    public required Guid VendorId { get; set; }
    public required Guid ContactTypeId { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }

    [Encrypted]
    public string? Phone { get; set; }

    [Encrypted]
    public string? Email { get; set; }

    public Vendor Vendor { get; set; } = default!;
    public ContactType ContactType { get; set; } = default!;
}
