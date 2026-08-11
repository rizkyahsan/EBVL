namespace EBVL.FrontEnd.Infrastructure.PathBase;

public static class ConfigurePathBase
{
    public static IApplicationBuilder UseAndCheckPathBase(this WebApplication app, string pathBase)
    {
        if (string.IsNullOrWhiteSpace(pathBase))
        {
            return app;
        }

        return app.UsePathBase(pathBase);
    }

    public static IApplicationBuilder MapBlazorHubWithPathBase(this WebApplication app, string pathBase)
    {
        if (string.IsNullOrWhiteSpace(pathBase))
        {
            return app;
        }

        _ = app.MapBlazorHub($"{pathBase}/_blazor");

        return app;
    }
}
