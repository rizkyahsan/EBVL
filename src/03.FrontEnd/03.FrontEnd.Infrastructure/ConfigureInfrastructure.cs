using EBVL.FrontEnd.Infrastructure.Authentication;
using EBVL.FrontEnd.Infrastructure.Authorization;
using EBVL.FrontEnd.Infrastructure.BackEndApi;
using EBVL.FrontEnd.Infrastructure.CurrentUser;
using EBVL.FrontEnd.Infrastructure.DateAndTime;
using EBVL.FrontEnd.Infrastructure.Emoji;
using EBVL.FrontEnd.Infrastructure.IdAMan;
using EBVL.FrontEnd.Infrastructure.Logging;
using EBVL.FrontEnd.Infrastructure.Monitoring;
using EBVL.FrontEnd.Infrastructure.Secret;
using EBVL.FrontEnd.Services.AppConfigFrontEnd;

namespace EBVL.FrontEnd.Infrastructure;

public static class ConfigureInfrastructure
{
    public static void AddInfrastructure(this WebApplicationBuilder builder, AppConfigFrontEndOptions appConfigFrontEndOptions, Dictionary<string, string> secrets)
    {
        var applicationInsightsConnectionString = secrets[SecretKeyFor.ConnectionStringsApplicationInsights];

        _ = builder.Host.AddLoggingService(applicationInsightsConnectionString);

        _ = builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
            options.KnownProxies.Clear();
            options.RequireHeaderSymmetry = false; // optional, helps with Azure
        });

        _ = builder.Services.AddHttpClient();
        _ = builder.Services.AddHttpContextAccessor();
        _ = builder.Services.AddMemoryCache();
        _ = builder.Services.AddAuthorizationService();
        _ = builder.Services.AddCurrentUserService();
        _ = builder.Services.AddDateAndTimeService();
        _ = builder.Services.AddMonitoringService(appConfigFrontEndOptions.AppNickName, applicationInsightsConnectionString);
        _ = builder.Services.AddBackEndApiService(appConfigFrontEndOptions.BackEndApiBaseUrl);

        var idAManClientId = secrets[SecretKeyFor.IdAManClientId];
        var idAManClientSecret = secrets[SecretKeyFor.IdAManClientSecret];
        var idAManObjectId = secrets[SecretKeyFor.IdAManObjectId];
        var idAManOptions = builder.Services.AddIdAManService(builder.Configuration, idAManClientId, idAManClientSecret, idAManObjectId);
        _ = builder.Services.AddAuthenticationService(idAManOptions);

        _ = builder.Services.AddEmojiService(builder.Configuration);
    }
}
