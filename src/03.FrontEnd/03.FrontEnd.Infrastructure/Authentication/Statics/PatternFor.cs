namespace EBVL.FrontEnd.Infrastructure.Authentication.Statics;

public static class PatternFor
{
    public const string Login = BaseRouteFor.Login;
    public const string Logout = BaseRouteFor.Logout;
    public const string SwitchPosition = $"{BaseRouteFor.SwitchPosition}/{{positionId}}";
    public const string LocalLoginPage = BaseRouteFor.LocalLoginPage;
    public const string LocalLoginHandler = $"{BaseRouteFor.LocalLoginHandler}/{{sessionId}}";
}

public static class BaseRouteFor
{
    public const string Login = nameof(Login);
    public const string Logout = nameof(Logout);
    public const string SwitchPosition = "Switch-Position";
    public const string LocalLoginPage = "Local-Login";
    public const string VerifyOtp = "Verify-Otp";
    public const string LocalLoginHandler = "Local-Login-Handler";
}
