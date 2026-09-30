using EBVL.BackEnd.Services.CurrentUser;
using Pertamina.Services.CurrentUser;
using Pertamina.Services.CurrentUser.HttpContext;

namespace EBVL.BackEnd.Infrastructure.CurrentUser;

public static class ConfigureCurrentUser
{
    public static IServiceCollection AddCurrentUserService(this IServiceCollection services)
    {
        _ = services.AddScoped<ICurrentUserService, HttpContextCurrentUserService>();
        _ = services.AddScoped<IRequestActor, HttpContextRequestActor>();

        return services;
    }
}
