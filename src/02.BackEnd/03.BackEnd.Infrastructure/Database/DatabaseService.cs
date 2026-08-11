using System.Reflection;
using Pertamina.Common.Domain.Attributes;
using Pertamina.Services.Cryptography;
using Pertamina.Services.Database.Common.ValueConverters;
using EBVL.BackEnd.Infrastructure.Database.Interceptors;

namespace EBVL.BackEnd.Infrastructure.Database;

public partial class DatabaseService(
    DbContextOptions<DatabaseService> options,
    AuditingSaveChangesInterceptor auditingSaveChangesInterceptor,
    ICryptographyService cryptographyService)
    : DbContext(options)
{
    public const string SchemaName = nameof(EBVL);
    public string CurrentActionName { get; private set; } = string.Empty;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(auditingSaveChangesInterceptor);

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.HasDefaultSchema(SchemaName);
        _ = modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        var stringType = typeof(string);
        var converter = new EncryptedConverter(cryptographyService);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.Name is not "Discriminator" && property.ClrType == stringType)
                {
                    var encryptedAttribute = property.PropertyInfo.GetCustomAttribute<EncryptedAttribute>();

                    if (encryptedAttribute is not null)
                    {
                        property.SetValueConverter(converter);
                    }
                }
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    public async Task<int> SaveAsync(string actionName, CancellationToken cancellationToken = default)
    {
        CurrentActionName = actionName;

        var numberOfStateEntries = await SaveChangesAsync(true, cancellationToken);

        CurrentActionName = string.Empty;

        return numberOfStateEntries;
    }
}
