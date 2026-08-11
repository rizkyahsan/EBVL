namespace EBVL.Shared.Dto.Modules.Main.Users.GetMyUser;

public sealed record UserItem
{
    public required Guid Id { get; init; }
    public required string Username { get; init; }
    public required string Name { get; init; }
    public required string EmailAddress { get; init; }
    public required string? PhoneNumber { get; init; }
    public required string QrCodeDataUri { get; init; }
    public bool IsVerified { get; init; }
}
