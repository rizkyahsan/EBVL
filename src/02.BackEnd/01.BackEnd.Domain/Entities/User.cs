namespace EBVL.BackEnd.Domain.Entities;

public sealed class User : ModifiableEntity
{
    public required Guid IdentityUserId { get; init; }
    public required Guid LenderId { get; init; }

    public required string Username { get; init; }
    public required string DisplayName { get; set; }
    public required string EmailAddress { get; set; }

    [ExcludeFromAudit]
    public string? PhoneCode { get; set; }
    [Encrypted]
    [ExcludeFromAudit]
    public string? PhoneNumber { get; set; }

    [ExcludeFromAudit]
    public string? AccessTokenHash { get; set; }
    [ExcludeFromAudit]
    public DateTimeOffset? AccessTokenExpiredAt { get; set; }

    public required string? OtpSecret { get; init; }
    public required string? OtpUrl { get; init; }
    public required bool IsVerified { get; set; }

    public required bool IsPicLender { get; set; }

    public Lender Lender { get; set; } = default!;

    public ICollection<ExternalLogin> ExternalLogins { get; set; } = new HashSet<ExternalLogin>();
}
