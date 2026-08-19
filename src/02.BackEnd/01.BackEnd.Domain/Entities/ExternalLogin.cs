namespace EBVL.BackEnd.Domain.Entities;

public sealed class ExternalLogin : ModifiableEntity
{
    public required Guid UserId { get; init; }
    public required Guid ExternalLoginLogId { get; init; }

    public required DateTimeOffset ExpiredAt { get; set; }
    public bool IsUsed { get; set; }

    public User User { get; set; } = default!;
    public ExternalLoginLog ExternalLoginLog { get; set; } = default!;
}
