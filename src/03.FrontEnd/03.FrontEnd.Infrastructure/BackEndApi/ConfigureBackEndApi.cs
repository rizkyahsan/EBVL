using Pertamina.Extensions.Polly;
using EBVL.FrontEnd.Services.BackEndApi;

namespace EBVL.FrontEnd.Infrastructure.BackEndApi;

public static class ConfigureBackEndApi
{
    private static readonly TimeSpan _handlerLifetime = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan _durationOfBreak = TimeSpan.FromSeconds(30);
    private const int RetryCount = 5;
    private const int AllowedHandledEvents = 5;

    public static IServiceCollection AddBackEndApiService(this IServiceCollection services, string backEndApiBaseUrl)
    {
        _ = services.AddHttpClient<IBackEndApiService, BackEndApiService>(client => client.BaseAddress = new Uri(backEndApiBaseUrl))
            .SetDefaultPollyPolicy(_handlerLifetime, RetryCount, AllowedHandledEvents, _durationOfBreak);

        return services;
    }
}
