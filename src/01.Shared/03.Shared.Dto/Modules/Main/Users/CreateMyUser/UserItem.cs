namespace EBVL.Shared.Dto.Modules.Main.Users.CreateMyUser;

public sealed record UserItem
{
    public required Guid Id { get; init; }
    public required string Username { get; init; }
}
