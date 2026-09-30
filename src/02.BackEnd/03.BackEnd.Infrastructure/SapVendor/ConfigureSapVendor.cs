using EBVL.BackEnd.Services.SapVendor;
using Pertamina.Extensions.Polly;

namespace EBVL.BackEnd.Infrastructure.SapVendor;

public static class ConfigureSapVendor
{
    public static IServiceCollection AddSapVendorService(this IServiceCollection services, IConfiguration configuration, string username, string password)
    {
        var configured = configuration.GetRequiredSection(SapVendorOptions.SectionKey).Get<SapVendorOptions>()
            ?? throw new ConfigurationBindingFailedException(SapVendorOptions.SectionKey, typeof(SapVendorOptions));
        var options = configured with { Username = username, Password = password };

        _ = services.AddSingleton(Options.Create(options));
        _ = services.AddHttpClient<ISapVendorService, SapVendorService>(client => client.Timeout = TimeSpan.FromSeconds(30))
            .SetDefaultPollyPolicy(TimeSpan.FromMinutes(5), 2, 5, TimeSpan.FromSeconds(30));

        return services;
    }
}
