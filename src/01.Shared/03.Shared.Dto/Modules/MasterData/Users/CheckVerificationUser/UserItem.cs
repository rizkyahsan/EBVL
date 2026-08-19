namespace EBVL.Shared.Dto.Modules.MasterData.Users.CheckVerificationUser;

public sealed record UserItem
{
    public required Guid Id { get; init; }
    public required string Username { get; init; }
    public required bool IsVerified { get; init; }
}
