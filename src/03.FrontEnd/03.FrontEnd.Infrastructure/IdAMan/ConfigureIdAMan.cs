using Pertamina.Extensions.Polly;
using Pertamina.Services.IdAMan;
using Pertamina.Services.PersonalRoles;
using Pertamina.Services.PersonalRoles.IdAMan;
using Pertamina.Services.PositionRoles;
using Pertamina.Services.PositionRoles.IdAMan;
using Pertamina.Services.UserPositions;
using Pertamina.Services.UserPositions.IdAMan;
using Pertamina.Services.UserProfile;
using Pertamina.Services.UserProfile.IdAMan;

namespace EBVL.FrontEnd.Infrastructure.IdAMan;

public static class ConfigureIdAMan
{
    private static readonly TimeSpan _handlerLifetime = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan _durationOfBreak = TimeSpan.FromSeconds(30);
    private const int RetryCount = 5;
    private const int AllowedHandledEvents = 5;

    public static IdAManOptions AddIdAManService(this IServiceCollection services, IConfiguration configuration, string clientId, string clientSecret, string objectId)
    {
        var idAManOptionsFromConfiguration = configuration.GetRequiredSection(IdAManOptions.SectionKey).Get<IdAManOptions>()
            ?? throw new ConfigurationBindingFailedException(IdAManOptions.SectionKey, typeof(IdAManOptions));

        var idAManOptions = new IdAManOptions
        {
            Authentication = idAManOptionsFromConfiguration.Authentication,
            RestApi = idAManOptionsFromConfiguration.RestApi,
            ClientId = clientId,
            ClientSecret = clientSecret,
            ObjectId = objectId
        };

        var tokenBaseUri = new Uri(idAManOptions.Authentication.AuthorityUrl);
        var apiBaseUri = new Uri($"{idAManOptions.RestApi.BaseUrl}{idAManOptions.RestApi.PathBase}");

        _ = services.AddSingleton(Options.Create(idAManOptions));
        _ = services.AddHttpClient<IdAManServiceProvider>(client => client.BaseAddress = tokenBaseUri)
            .SetDefaultPollyPolicy(_handlerLifetime, RetryCount, AllowedHandledEvents, _durationOfBreak);

        _ = services.Configure<IdAManPersonalRolesOptions>(configuration.GetRequiredSection(IdAManPersonalRolesOptions.SectionKey));
        _ = services.AddHttpClient<IPersonalRolesService, IdAManPersonalRolesService>(client => client.BaseAddress = apiBaseUri)
            .SetDefaultPollyPolicy(_handlerLifetime, RetryCount, AllowedHandledEvents, _durationOfBreak);

        _ = services.Configure<IdAManPositionRolesOptions>(configuration.GetRequiredSection(IdAManPositionRolesOptions.SectionKey));
        _ = services.AddHttpClient<IPositionRolesService, IdAManPositionRolesService>(client => client.BaseAddress = apiBaseUri)
            .SetDefaultPollyPolicy(_handlerLifetime, RetryCount, AllowedHandledEvents, _durationOfBreak);

        _ = services.Configure<IdAManUserPositionsOptions>(configuration.GetRequiredSection(IdAManUserPositionsOptions.SectionKey));
        _ = services.AddHttpClient<IUserPositionsService, IdAManUserPositionsService>(client => client.BaseAddress = apiBaseUri)
            .SetDefaultPollyPolicy(_handlerLifetime, RetryCount, AllowedHandledEvents, _durationOfBreak);

        _ = services.Configure<IdAManUserProfileOptions>(configuration.GetRequiredSection(IdAManUserProfileOptions.SectionKey));
        _ = services.AddHttpClient<IUserProfileService, IdAManUserProfileService>(client => client.BaseAddress = apiBaseUri)
            .SetDefaultPollyPolicy(_handlerLifetime, RetryCount, AllowedHandledEvents, _durationOfBreak);

        return idAManOptions;
    }
}
