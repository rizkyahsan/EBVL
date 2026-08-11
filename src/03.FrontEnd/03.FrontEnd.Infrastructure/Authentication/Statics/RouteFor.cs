namespace EBVL.FrontEnd.Infrastructure.Authentication.Statics;

public static class RouteFor
{
    public const string Landing = nameof(Landing);
    public const string AccessDenied = nameof(AccessDenied);

    public static string Login(string? returnUrl = null)
    {
        return $"{PrefixFor.Authentication}/{PatternFor.Login}?{QueryStringFor.ReturnUrl}={returnUrl}";
    }

    public static string Logout(string? returnUrl = null)
    {
        return $"{PrefixFor.Authentication}/{PatternFor.Logout}?{QueryStringFor.ReturnUrl}={returnUrl}";
    }

    public static string SwitchPosition(string positionId, string? returnUrl = null)
    {
        return $"{PrefixFor.Authentication}/SwitchPosition/{positionId}?{QueryStringFor.ReturnUrl}={returnUrl}";
    }
}
