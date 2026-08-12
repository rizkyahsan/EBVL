namespace EBVL.BackEnd.Domain.Entities;

public sealed class Vendor : ModifiableEntity
{
    public required string SapVendorNumber { get; set; }
    public required string Name { get; set; }

    [Encrypted]
    public required string Email { get; set; }

    [Encrypted]
    public string? TaxId { get; set; }

    public string? Website { get; set; }
    public Guid? VendorTypeId { get; set; }
    public string? LegacyConfirmedStatus { get; set; }
    public VendorType? VendorType { get; set; }
    public ICollection<VendorContact> Contacts { get; set; } = [];
    public ICollection<VendorDocument> Documents { get; set; } = [];
    public ICollection<VendorAccount> Accounts { get; set; } = [];
    public ICollection<VendorRegistration> Registrations { get; set; } = [];
}
