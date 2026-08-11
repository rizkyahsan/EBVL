using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pertamina.Extensions.Polly;
using Pertamina.Services.Email;
using Pertamina.Services.Email.EmailBlast;
using Pertamina.Services.Email.None;
using Pertamina.Services.Email.Smtp;

namespace EBVL.BackEnd.Infrastructure.Email;

public static class ConfigureEmail
{
    private static readonly TimeSpan _handlerLifetime = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan _durationOfBreak = TimeSpan.FromSeconds(30);
    private const int RetryCount = 5;
    private const int AllowedHandledEvents = 5;

    public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration, string smtpUsername, string smtpPassword, IHealthChecksBuilder healthChecksBuilder)
    {
        var emailProvider = configuration[$"{nameof(Email)}:Provider"]
            ?? throw new ConfigurationBindingFailedException($"{nameof(Email)}:Provider", typeof(string));

        _ = services.Configure<EmailOptions>(configuration.GetRequiredSection(EmailOptions.SectionKey));

        switch (emailProvider)
        {
            case EmailProvider.Smtp:
                var smtpEmailOptionsFromConfiguration = configuration.GetRequiredSection(SmtpEmailOptions.SectionKey).Get<SmtpEmailOptions>()
                    ?? throw new ConfigurationBindingFailedException(SmtpEmailOptions.SectionKey, typeof(SmtpEmailOptions));

                var smtpEmailOptions = new SmtpEmailOptions
                {
                    Server = smtpEmailOptionsFromConfiguration.Server,
                    Port = smtpEmailOptionsFromConfiguration.Port,
                    Username = smtpUsername,
                    Password = smtpPassword,
                    SecureOption = smtpEmailOptionsFromConfiguration.SecureOption
                };

                _ = services.AddSingleton(Options.Create(smtpEmailOptions));
                _ = services.AddTransient<SmtpEmailSender>();
                _ = services.AddTransient<IEmailService, SmtpEmailService>();

                _ = healthChecksBuilder.AddSmtpHealthCheck(options =>
                    {
                        options.Host = smtpEmailOptions.Server;
                        options.Port = smtpEmailOptions.Port;
                        options.AllowInvalidRemoteCertificates = true;
                    },
                    name: $"Email Service: SMTP ({smtpEmailOptions.Server}:{smtpEmailOptions.Port})",
                    failureStatus: HealthStatus.Degraded,
                    tags: ["Email"]);
                break;
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
            case EmailProvider.None:
                _ = services.AddTransient<IEmailService, NoneEmailService>();
                break;
            default:
                throw new UnsupportedServiceProviderException(nameof(Email), emailProvider);
        }

        return services;
    }
}
