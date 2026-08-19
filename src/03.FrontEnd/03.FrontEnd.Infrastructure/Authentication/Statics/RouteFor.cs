namespace EBVL.FrontEnd.Infrastructure.Authentication.Statics;

public static class RouteFor
{
    public const string Landing = nameof(Landing);
    public const string AccessDenied = nameof(AccessDenied);

    public static string Login(string? returnUrl = null)
    {
        return $"{PrefixFor.Authentication}/{BaseRouteFor.Login}?{QueryStringFor.ReturnUrl}={returnUrl}";
    }

    public static string LocalLoginPage(string? returnUrl = null)
    {
        return $"{PrefixFor.Authentication}/{BaseRouteFor.LocalLoginPage}?{QueryStringFor.ReturnUrl}={returnUrl}";
    }

    public static string Logout(string? returnUrl = null)
    {
        return $"{PrefixFor.Authentication}/{BaseRouteFor.Logout}?{QueryStringFor.ReturnUrl}={returnUrl}";
    }

    public static string SwitchPosition(string positionId, string? returnUrl = null)
    {
        return $"{PrefixFor.Authentication}/{BaseRouteFor.SwitchPosition}/{positionId}?{QueryStringFor.ReturnUrl}={returnUrl}";
    }

    public static string VerifyOtp(string username, string? returnUrl = null)
    {
        return $"{PrefixFor.Authentication}/{BaseRouteFor.VerifyOtp}/{username}?{QueryStringFor.ReturnUrl}={returnUrl}";
    }

    public static string LocalLoginHandler(string sessionId, string? returnUrl = null)
    {
        return $"{PrefixFor.Authentication}/{BaseRouteFor.LocalLoginHandler}/{sessionId}?{QueryStringFor.ReturnUrl}={returnUrl}";
    }
}
