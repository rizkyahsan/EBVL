namespace EBVL.FrontEnd.Infrastructure.Authentication.Statics;

public static class RouteFor
{
    public const string Landing = nameof(Landing);
    public const string AccessDenied = nameof(AccessDenied);

    public static string Login(string? returnUrl = null)
    {
        return WithReturnUrl($"{PrefixFor.Authentication}/{BaseRouteFor.Login}", returnUrl);
    }

    public static string VendorLoginPage(string? returnUrl = null)
    {
        return WithReturnUrl($"{PrefixFor.Authentication}/{BaseRouteFor.VendorLoginPage}", returnUrl);
    }

    public static string Logout(string? returnUrl = null)
    {
        return WithReturnUrl($"{PrefixFor.Authentication}/{BaseRouteFor.Logout}", returnUrl);
    }

    public static string SwitchPosition(string positionId, string? returnUrl = null)
    {
        return WithReturnUrl($"{PrefixFor.Authentication}/{BaseRouteFor.SwitchPosition}/{positionId}", returnUrl);
    }

    public static string VerifyOtp(string username, string? returnUrl = null)
    {
        return WithReturnUrl($"{PrefixFor.Authentication}/{BaseRouteFor.VerifyOtp}/{username}", returnUrl);
    }

    public static string LocalLoginHandler(string sessionId, string? returnUrl = null)
    {
        return WithReturnUrl($"{PrefixFor.Authentication}/{BaseRouteFor.LocalLoginHandler}/{sessionId}", returnUrl);
    }

    private static string WithReturnUrl(string route, string? returnUrl)
    {
        return string.IsNullOrWhiteSpace(returnUrl) ? route : $"{route}?{QueryStringFor.ReturnUrl}={Uri.EscapeDataString(returnUrl)}";
    }
}
