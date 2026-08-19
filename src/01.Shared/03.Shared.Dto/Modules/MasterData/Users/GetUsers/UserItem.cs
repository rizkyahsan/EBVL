namespace EBVL.Shared.Dto.Modules.MasterData.Users.GetUsers;

public sealed record UserItem
{
    public required Guid Id { get; init; }
    public required Guid LenderId { get; init; }
    public required string LenderName { get; init; }
    public required string Username { get; init; }
    public required string Name { get; init; }
    public required string Lender { get; init; }
    public required string FullPhoneNumber { get; init; }
    public required string PhoneCode { get; init; }
    public required string PhoneNumber { get; init; }
    public required string EmailAddress { get; init; }
    public bool IsVerified { get; init; }
    public bool IsPicLender { get; set; }
    public int CountPicLender { get; set; }
}
