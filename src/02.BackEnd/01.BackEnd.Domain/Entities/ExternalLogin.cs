namespace EBVL.BackEnd.Domain.Entities;

public sealed class ExternalLogin : ModifiableEntity
{
    public Guid? UserId { get; set; }
    public string? Username { get; set; }
    public bool IsSuccess { get; set; }
    public string? FailureReason { get; set; }
    public DateTimeOffset? AttemptedAt { get; set; }
    public DateTimeOffset? VerifiedAt { get; set; }
    public string? IpAddress { get; set; }

    public DateTimeOffset ExpiredAt { get; set; }
    public bool IsUsed { get; set; }

    public User? User { get; set; }
}
