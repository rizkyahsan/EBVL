using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace EBVL.FrontEnd.Infrastructure.Logging;

public static class ConfigureLogging
{
    private const string OutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}";

    public static IHostBuilder AddLoggingService(this IHostBuilder hostBuilder, string connectionString)
    {
        _ = hostBuilder.UseSerilog((hostBuilderContext, loggerConfiguration) =>
        {
            _ = loggerConfiguration
                .ReadFrom.Configuration(hostBuilderContext.Configuration)
                .WriteTo.Console(outputTemplate: OutputTemplate, theme: AnsiConsoleTheme.Code);

            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                _ = loggerConfiguration.WriteTo.ApplicationInsights(connectionString, TelemetryConverter.Traces, restrictedToMinimumLevel: LogEventLevel.Information);
            }
        });

        SelfLog.Enable(Console.WriteLine);

        return hostBuilder;
    }
}
