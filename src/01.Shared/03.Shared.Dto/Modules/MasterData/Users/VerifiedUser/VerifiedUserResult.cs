namespace EBVL.Shared.Dto.Modules.MasterData.Users.VerifiedUser;

public sealed record VerifiedUserResult
{
    public bool Succeeded { get; init; }
    public string? ErrorMessage { get; init; }
}
