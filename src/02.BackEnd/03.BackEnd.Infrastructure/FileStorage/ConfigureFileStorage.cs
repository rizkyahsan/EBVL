using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pertamina.Services.FileStorage;
using Pertamina.Services.FileStorage.FileSystem;

namespace EBVL.BackEnd.Infrastructure.FileStorage;

public static class ConfigureFileStorage
{
    public static IServiceCollection AddFileStorageService(this IServiceCollection services, IConfiguration configuration, IHealthChecksBuilder healthChecksBuilder)
    {
        var fileSystemFileStorageSection = configuration.GetRequiredSection(FileSystemFileStorageOptions.SectionKey);
        var fileSystemFileStorageOptions = fileSystemFileStorageSection.Get<FileSystemFileStorageOptions>()
            ?? throw ExceptionFor.ConfigurationBindingFailed(FileSystemFileStorageOptions.SectionKey, typeof(FileSystemFileStorageOptions));

        _ = services.Configure<FileSystemFileStorageOptions>(fileSystemFileStorageSection);
        _ = services.AddSingleton<IFileStorageService, FileSystemFileStorageService>();

        if (!Directory.Exists(fileSystemFileStorageOptions.FolderPath))
        {
            _ = Directory.CreateDirectory(fileSystemFileStorageOptions.FolderPath);
        }

        _ = healthChecksBuilder.Add(new HealthCheckRegistration(
            name: $"File Storage Service: File System ({fileSystemFileStorageOptions.FolderPath})",
            instance: new FileSystemFileStorageHealthCheck(fileSystemFileStorageOptions.FolderPath),
            failureStatus: HealthStatus.Unhealthy,
            tags: ["File Storage"]));

        return services;
    }
}
