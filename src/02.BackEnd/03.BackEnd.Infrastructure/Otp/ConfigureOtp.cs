using Pertamina.Services.Otp;
using Pertamina.Services.Otp.Default;

namespace EBVL.BackEnd.Infrastructure.Otp;

public static class ConfigureOtp
{
    public static IServiceCollection AddOtpService(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.Configure<OtpOptions>(configuration.GetRequiredSection(OtpOptions.SectionKey));
        _ = services.AddTransient<IOtpService, DefaultOtpService>();

        return services;
    }
}
