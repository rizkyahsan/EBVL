using EBVL.BackEnd.Infrastructure.Authentication;
using EBVL.BackEnd.Infrastructure.BackgroundJob;
using EBVL.BackEnd.Infrastructure.Cryptography;
using EBVL.BackEnd.Infrastructure.CurrentUser;
using EBVL.BackEnd.Infrastructure.Database;
using EBVL.BackEnd.Infrastructure.DateAndTime;
using EBVL.BackEnd.Infrastructure.Email;
using EBVL.BackEnd.Infrastructure.Endpoint;
using EBVL.BackEnd.Infrastructure.ExceptionHandler;
using EBVL.BackEnd.Infrastructure.FileStorage;
using EBVL.BackEnd.Infrastructure.HealthCheck;
using EBVL.BackEnd.Infrastructure.IdAMan;
using EBVL.BackEnd.Infrastructure.Logging;
using EBVL.BackEnd.Infrastructure.Monitoring;
using EBVL.BackEnd.Infrastructure.Otp;
using EBVL.BackEnd.Infrastructure.PublicHolidays;
using EBVL.BackEnd.Infrastructure.Secret;
using EBVL.BackEnd.Services.AppConfigBackEnd;

namespace EBVL.BackEnd.Infrastructure;

public static class ConfigureInfrastructure
{
    public static void AddInfrastructure(this WebApplicationBuilder builder, AppConfigBackEndOptions appConfigBackEndOptions, Dictionary<string, string> secrets)
    {
        var applicationInsightsConnectionString = secrets[SecretKeyFor.ConnectionStringsApplicationInsights];
        var applicationDatabaseConnectionString = secrets[SecretKeyFor.ConnectionStringsApplicationDatabase];
        var servicesDatabaseConnectionString = secrets[SecretKeyFor.ConnectionStringsServicesDatabase];
        var cryptographyKey = secrets[SecretKeyFor.CryptographyKey];
        var cryptographyTweak = secrets[SecretKeyFor.CryptographyTweak];
        var smtpUsername = secrets[SecretKeyFor.EmailSmtpUsername];
        var smtpPassword = secrets[SecretKeyFor.EmailSmtpKataKunci];

        _ = builder.Host.AddLoggingService(applicationInsightsConnectionString);

        var healthChecksBuilder = builder.Services.AddHealthCheckService(builder.Configuration, appConfigBackEndOptions.PathBase, appConfigBackEndOptions.AppNickName, servicesDatabaseConnectionString);

        _ = builder.Services.AddHttpClient();
        _ = builder.Services.AddHttpContextAccessor();
        _ = builder.Services.AddBackgroundJobService(builder.Configuration, servicesDatabaseConnectionString, healthChecksBuilder);
        _ = builder.Services.AddCryptographyService(cryptographyKey, cryptographyTweak);
        _ = builder.Services.AddCurrentUserService();
        _ = builder.Services.AddDatabaseService(applicationDatabaseConnectionString, healthChecksBuilder);
        _ = builder.Services.AddDateAndTimeService();
        _ = builder.Services.AddEmailService(builder.Configuration, smtpUsername, smtpPassword, healthChecksBuilder);
        _ = builder.Services.AddExceptionHandlerService();
        _ = builder.Services.AddFileStorageService(builder.Configuration, healthChecksBuilder);
        _ = builder.Services.AddMonitoringService(appConfigBackEndOptions.AppNickName, applicationInsightsConnectionString);
        _ = builder.Services.AddOtpService(builder.Configuration);
        _ = builder.Services.AddAuthorization();
        _ = builder.Services.AddOpenApi(options => _ = options.AddSchemaTransformer(new CustomSchemaTransformer()));

        var idAManClientId = secrets[SecretKeyFor.IdAManClientId];
        var idAManClientSecret = secrets[SecretKeyFor.IdAManClientSecret];
        var idAManObjectId = secrets[SecretKeyFor.IdAManObjectId];
        var idAManOptions = builder.Services.AddIdAManService(builder.Configuration, idAManClientId, idAManClientSecret, idAManObjectId, healthChecksBuilder);
        _ = builder.Services.AddAuthenticationService(idAManOptions);

        _ = builder.Services.AddPublicHolidaysService(builder.Configuration);
    }
}
