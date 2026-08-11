using Pertamina.Services.CurrentUser;
using Pertamina.Services.CurrentUser.HttpContext;

namespace EBVL.FrontEnd.Infrastructure.CurrentUser;

public static class ConfigureCurrentUser
{
    public static IServiceCollection AddCurrentUserService(this IServiceCollection services)
    {
        _ = services.AddScoped<ICurrentUserService, HttpContextCurrentUserService>();

        return services;
    }
}
