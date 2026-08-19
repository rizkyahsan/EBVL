namespace EBVL.Shared.Dto.Modules.MasterData.Users.ResetPasswordUser;

public sealed record ResetPasswordUserResult
{
    public bool Succeeded { get; init; }
    public string? ErrorMessage { get; init; }
}
