namespace EBVL.FrontEnd.Infrastructure.Authentication.Statics;

public static class PatternFor
{
    public const string Login = nameof(Login);
    public const string LocalLogin = nameof(LocalLogin);
    public const string Logout = nameof(Logout);
    public const string SwitchPosition = $"{nameof(SwitchPosition)}/{{positionId}}";
}
