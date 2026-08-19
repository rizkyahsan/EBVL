namespace EBVL.BackEnd.Domain.Entities;

public sealed class ExternalLoginLog : ModifiableEntity
{
    public Guid? UserId { get; set; }

    public required string Username { get; set; }
    public required bool IsSuccess { get; set; }
    public string? FailureReason { get; set; }
    public required DateTimeOffset? AttemptedAt { get; set; }
    public DateTimeOffset? VerifiedAt { get; set; }
    public string? IpAddress { get; set; }

    public ExternalLogin? ExternalLogin { get; set; }
}
