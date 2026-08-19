using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace EBVL.BackEnd.Infrastructure.Authentication;

public sealed class CustomJwtBearerEvents(ILogger<CustomJwtBearerEvents> logger) : JwtBearerEvents
{
    public override Task AuthenticationFailed(AuthenticationFailedContext context)
    {
        logger.LogError(context.Exception, "JWT Bearer Authentication failed. {ErrorMessage}", context.Exception.Message);

        return Task.CompletedTask;
    }

    public override Task MessageReceived(MessageReceivedContext context)
    {
        logger.LogInformation("JWT Bearer Message received. Request Path: {RequestPath}", context.HttpContext.Request.Path);

        return Task.CompletedTask;
    }
}
