namespace EBVL.BackEnd.Domain.Entities;

public sealed class VendorAccount : ModifiableEntity
{
    [Encrypted]
    public required string EmailAddress { get; set; }

    [ExcludeFromAudit]
    public required string PasswordHash { get; set; }

    [ExcludeFromAudit]
    public required string PasswordSalt { get; set; }

    public required bool IsActive { get; set; }
    public required Guid VendorRegistrationId { get; set; }
    public VendorRegistration VendorRegistration { get; set; } = default!;
}
