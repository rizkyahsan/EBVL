namespace EBVL.FrontEnd.Infrastructure.PathBase;

public static class ConfigurePathBase
{
    public static IApplicationBuilder UseAndCheckPathBase(this WebApplication app, string pathBase)
    {
        if (string.IsNullOrWhiteSpace(pathBase))
        {
            return app;
        }

        if (app.Logger.IsEnabled(LogLevel.Information))
        {
            app.Logger.LogInformation("This web application is configured with Path Base: {PathBase}.", pathBase);
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
