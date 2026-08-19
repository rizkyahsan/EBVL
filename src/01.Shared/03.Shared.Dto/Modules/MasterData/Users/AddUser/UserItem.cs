namespace EBVL.Shared.Dto.Modules.MasterData.Users.AddUser;

public sealed record UserItem
{
    public required Guid Id { get; init; }
    public required string Username { get; init; }
}
