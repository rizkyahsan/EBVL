using EBVL.BackEnd.Services.EmailBlast2;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pertamina.Extensions.Polly;
using Pertamina.Services.Email;
using Pertamina.Services.Email.EmailBlast;
using Pertamina.Services.Email.None;

namespace EBVL.BackEnd.Infrastructure.Email;

public static class ConfigureEmail
{
    private static readonly TimeSpan _handlerLifetime = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan _durationOfBreak = TimeSpan.FromSeconds(30);
    private const int RetryCount = 5;
    private const int AllowedHandledEvents = 5;

    public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration, IHealthChecksBuilder healthChecksBuilder, string emailBlastClientId, string emailBlastClientSecret, string sendGridApiKey)
    {
        var emailProvider = configuration[$"{nameof(Email)}:Provider"]
            ?? throw new ConfigurationBindingFailedException($"{nameof(Email)}:Provider", typeof(string));

        _ = services.Configure<EmailOptions>(configuration.GetRequiredSection(EmailOptions.SectionKey));

        switch (emailProvider)
        {
            case EmailProvider.EmailBlast:
                var emailBlastEmailSection = configuration.GetRequiredSection(EmailBlastEmailOptions.SectionKey);
                var emailBlastEmailOptions = emailBlastEmailSection.Get<EmailBlastEmailOptions>()
                    ?? throw new ConfigurationBindingFailedException(EmailBlastEmailOptions.SectionKey, typeof(EmailBlastEmailOptions));

                _ = services.Configure<EmailBlastEmailOptions>(emailBlastEmailSection);
                _ = services.AddHttpClient<IEmailService, EmailBlastEmailService>()
                    .SetDefaultPollyPolicy(_handlerLifetime, RetryCount, AllowedHandledEvents, _durationOfBreak);

                _ = healthChecksBuilder.Add(new HealthCheckRegistration(
                    name: $"Email Service: Email Blast ({emailBlastEmailOptions.ApiBaseUrl})",
                    instance: new EmailBlastEmailServiceHealthCheck(emailBlastEmailOptions.RestBaseUrl, emailBlastEmailOptions.HealthCheckEndpoint),
                    failureStatus: HealthStatus.Unhealthy,
                    tags: ["Email"]));
                break;
            case EmailProvider.EmailBlast2:
                var emailBlast2EmailFromConfiguration = configuration.GetRequiredSection(EmailBlast2EmailOptions.SectionKey).Get<EmailBlast2EmailOptions>()
                    ?? throw new ConfigurationBindingFailedException(EmailBlast2EmailOptions.SectionKey, typeof(EmailBlast2EmailOptions));

                var emailBlast2EmailOptions = new EmailBlast2EmailOptions
                {
                    Scope = emailBlast2EmailFromConfiguration.Scope,
                    RestBaseUrl = emailBlast2EmailFromConfiguration.RestBaseUrl,
                    TokenEndpoint = emailBlast2EmailFromConfiguration.TokenEndpoint,
                    ClientId = emailBlastClientId,
                    ClientSecret = emailBlastClientSecret,
                    HealthCheckEndpoint = emailBlast2EmailFromConfiguration.HealthCheckEndpoint,
                    ApiPathBase = emailBlast2EmailFromConfiguration.ApiPathBase,
                    ResourceEndpoint = emailBlast2EmailFromConfiguration.ResourceEndpoint,
                    SendGridApiKey = sendGridApiKey,
                };

                _ = services.AddSingleton(Options.Create(emailBlast2EmailOptions));
                _ = services.AddHttpClient<IEmailBlast2Service, EmailBlast2EmailService>()
                    .SetDefaultPollyPolicy(_handlerLifetime, RetryCount, AllowedHandledEvents, _durationOfBreak);

                _ = healthChecksBuilder.Add(new HealthCheckRegistration(
                    name: $"Email Service: Email Blast2 ({emailBlast2EmailOptions.ApiBaseUrl})",
                    instance: new EmailBlastEmailServiceHealthCheck(emailBlast2EmailOptions.RestBaseUrl, emailBlast2EmailOptions.HealthCheckEndpoint),
                    failureStatus: HealthStatus.Unhealthy,
                    tags: ["Email"]));
                break;
            case EmailProvider.None:
                _ = services.AddTransient<IEmailService, NoneEmailService>();
                break;
            default:
                throw new UnsupportedServiceProviderException(nameof(Email), emailProvider);
        }

        return services;
    }
}
