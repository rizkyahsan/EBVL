using System.Text.Json;
using Microsoft.EntityFrameworkCore.Design;
using EBVL.BackEnd.Infrastructure.Cryptography;
using EBVL.BackEnd.Infrastructure.CurrentUser;
using EBVL.BackEnd.Infrastructure.DateAndTime;
using EBVL.BackEnd.Infrastructure.Database.Interceptors;
using EBVL.BackEnd.Infrastructure.Secret;

namespace EBVL.BackEnd.Infrastructure.Database;

public sealed class DatabaseServiceDesignTimeFactory : IDesignTimeDbContextFactory<DatabaseService>
{
    public DatabaseService CreateDbContext(string[] args)
    {
        var secretsPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "secrets.json"));
        using var document = JsonDocument.Parse(File.ReadAllText(secretsPath));
        var secrets = document.RootElement;

        var services = new ServiceCollection();
        _ = services.AddHttpContextAccessor();
        _ = services.AddCurrentUserService();
        _ = services.AddDateAndTimeService();
        _ = services.AddCryptographyService(
            GetRequiredSecret(secrets, SecretKeyFor.CryptographyKey),
            GetRequiredSecret(secrets, SecretKeyFor.CryptographyTweak));
        _ = services.AddScoped<AuditingSaveChangesInterceptor>();

        var connectionString = GetRequiredSecret(secrets, SecretKeyFor.ConnectionStringsApplicationDatabase);
        _ = services.AddDbContext<DatabaseService>(options => options.UseSqlServer(connectionString, builder =>
        {
            _ = builder.MigrationsAssembly(typeof(DatabaseService).Assembly.FullName);
            _ = builder.MigrationsHistoryTable("__EFMigrationsHistory", nameof(EBVL));
            _ = builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        }));

        return services.BuildServiceProvider().GetRequiredService<DatabaseService>();
    }

    private static string GetRequiredSecret(JsonElement secrets, string key)
    {
        if (!secrets.TryGetProperty(key, out var value) || string.IsNullOrWhiteSpace(value.GetString()))
        {
            throw new InvalidOperationException($"Secret design-time '{key}' tidak ditemukan.");
        }

        return value.GetString()!;
    }
}
