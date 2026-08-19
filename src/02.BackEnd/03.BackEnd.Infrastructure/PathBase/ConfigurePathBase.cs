using Microsoft.AspNetCore.Http;

namespace EBVL.BackEnd.Infrastructure.PathBase;

public static class ConfigurePathBase
{
    public static IApplicationBuilder UseAndCheckPathBase(this WebApplication app, string pathBase)
    {
        if (string.IsNullOrWhiteSpace(pathBase))
        {
            return app;
        }

        _ = app.UsePathBase(pathBase);

        _ = app.Use(async (context, next) =>
        {
            if (context.Request.PathBase != pathBase)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;

                return;
            }

            await next();
        });

        if (app.Logger.IsEnabled(LogLevel.Information))
        {
            app.Logger.LogInformation("This web application is configured with Path Base: {PathBase}.", pathBase);
        }

        return app;
    }
}
