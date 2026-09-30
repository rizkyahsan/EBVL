namespace EBVL.BackEnd.Services.CurrentUser;

public interface IRequestActor
{
    public string Username { get; }
    public IReadOnlySet<string> Roles { get; }
    public IReadOnlySet<string> Scopes { get; }
}
