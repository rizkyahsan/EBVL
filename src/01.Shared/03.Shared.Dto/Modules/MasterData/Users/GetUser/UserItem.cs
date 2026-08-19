namespace EBVL.Shared.Dto.Modules.MasterData.Users.GetUser;

public sealed record UserItem
{
    public required Guid Id { get; init; }
    public required DateTimeOffset Created { get; init; }
    public required string CreatedBy { get; init; }
    public required DateTimeOffset? Modified { get; init; }
    public required string? ModifiedBy { get; init; }

    public required Guid LenderId { get; init; }
    public required string LenderName { get; init; }
    public required string Username { get; init; }
    public required string Name { get; init; }
    public required string FullPhoneNumber { get; init; }
    public required string PhoneCode { get; init; }
    public required string PhoneNumber { get; init; }
    public required string EmailAddress { get; init; }
    public bool IsVerified { get; init; }
    public bool IsPicLender { get; init; }
    public int CountPicLender { get; set; }

    public required IEnumerable<AuditItem> Audits { get; init; }
}
