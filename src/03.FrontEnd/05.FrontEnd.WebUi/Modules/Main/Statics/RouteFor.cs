using System.Net;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Statics;

public static class RouteFor
{
    public const string Index = "";
    public const string Landing = nameof(Landing);
    public const string AccessDenied = nameof(AccessDenied);
    public const string About = nameof(About);
    public const string Error = nameof(Error);
    public const string MySession = nameof(MySession);

    public static string ErrorWithCode(HttpStatusCode statusCode)
    {
        return $"ErrorWithCode/{(int)statusCode}";
    }

    public static string ErrorWithCode(int statusCode)
    {
        return $"ErrorWithCode/{statusCode}";
    }
}
