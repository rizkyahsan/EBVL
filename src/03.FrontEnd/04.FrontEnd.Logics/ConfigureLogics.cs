using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pertamina.Extensions.MediatR.Behaviors;

namespace EBVL.FrontEnd.Logics;

public static class ConfigureLogics
{
    public static void AddLogics(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.Configure<LoggingBehaviorOptions>(configuration.GetSection(LoggingBehaviorOptions.SectionKey));
        _ = services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        _ = services.AddMediatR(configuration =>
        {
            _ = configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            _ = configuration.AddOpenRequestPreProcessor(typeof(LoggingBehavior<>));
            _ = configuration.AddOpenBehavior(typeof(ExceptionBehavior<,>));
            _ = configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            _ = configuration.AddOpenBehavior(typeof(PerformanceBehavior<,>));
        });
    }
}
