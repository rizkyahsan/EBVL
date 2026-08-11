using Pertamina.Services.DateAndTime;
using Pertamina.Services.DateAndTime.System;

namespace EBVL.BackEnd.Infrastructure.DateAndTime;

public static class ConfigureDateAndTime
{
    public static IServiceCollection AddDateAndTimeService(this IServiceCollection services)
    {
        _ = services.AddSingleton<IDateAndTimeService, SystemDateAndTimeService>();

        return services;
    }
}
