using EBVL.BackEnd.Services.PublicHolidays;

namespace EBVL.BackEnd.Infrastructure.PublicHolidays;

public static class ConfigurePublicHolidays
{
    public static IServiceCollection AddPublicHolidaysService(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.Configure<PublicHolidaysOptions>(configuration.GetRequiredSection(PublicHolidaysOptions.SectionKey));
        _ = services.AddScoped<IPublicHolidaysService, PublicHolidaysService>();

        return services;
    }
}
