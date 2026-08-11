namespace EBVL.BackEnd.Domain.Entities;

public sealed class User : ModifiableEntity
{
    public required string Username { get; init; }
    public required string Name { get; set; }

    [Encrypted]
    public required string EmailAddress { get; set; }

    [Encrypted]
    [ExcludeFromAudit]
    public required string? PhoneNumber { get; set; }

    public required string OtpSecret { get; init; }
    public required string OtpUrl { get; init; }
    public required bool IsVerified { get; set; }
}
