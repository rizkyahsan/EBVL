using System.Security.Claims;
using EBVL.BackEnd.Infrastructure.LocalIdentity.InitialData;
using EBVL.BackEnd.Infrastructure.LocalIdentity.Migrator;
using EBVL.BackEnd.Infrastructure.LocalIdentity.Models;
using EBVL.BackEnd.Infrastructure.LocalIdentity.Statics;
using EBVL.BackEnd.Services.LocalIdentity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EBVL.BackEnd.Infrastructure.LocalIdentity;

public static class ConfigureLocalIdentity
{
    public static IServiceCollection AddLocalIdentityService(this IServiceCollection services, string connectionString)
    {
        _ = services.AddDbContext<AspNetLocalIdentityDatabase>(options =>
        {
            _ = options.UseSqlServer(connectionString, builder =>
            {
                _ = builder.MigrationsAssembly(typeof(AspNetLocalIdentityDatabase).Assembly.FullName);
                _ = builder.MigrationsHistoryTable(TableNameFor.EfMigrationsHistory, SchemaNameFor.LocalIdentity);
                _ = builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });

            _ = options.ConfigureWarnings(wcb => wcb.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning));
            _ = options.ConfigureWarnings(wcb => wcb.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
        });

        _ = services.AddIdentityCore<AspNetCoreUser>()
            .AddRoles<AspNetCoreRole>()
            .AddEntityFrameworkStores<AspNetLocalIdentityDatabase>()
            .AddDefaultTokenProviders();

        _ = services.Configure<IdentityOptions>(options => options.User.RequireUniqueEmail = true);
        _ = services.AddScoped<ILocalIdentityService, AspNetLocalIdentityService>();

        _ = services.AddScoped<LocalIdentityMigrator>();

        return services;
    }

    public static async Task InitializeLocalIdentityDatabase(this IHost host)
    {
        using var serviceScope = host.Services.CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;

        var localIdentityMigrator = serviceProvider.GetRequiredService<LocalIdentityMigrator>();
        await localIdentityMigrator.MigrateAsync();

        await SeedRolesAndClaimsAsync(serviceProvider);
    }

    /// <summary>
    /// Seeds initial roles and their claims into the ASP.NET Core Identity system.
    /// This method ensures that all roles defined in InitialRoles.All exist,
    /// and attaches any claims defined in InitialRoleClaim.All to those roles.
    /// </summary>
    /// <param name="serviceProvider">
    /// Provides access to required services such as RoleManager and DbContext.
    /// </param>
    private static async Task SeedRolesAndClaimsAsync(IServiceProvider serviceProvider)
    {
        // Resolve dependencies from the DI container
        var roleManager = serviceProvider.GetRequiredService<RoleManager<AspNetCoreRole>>();
        var dbContext = serviceProvider.GetRequiredService<AspNetLocalIdentityDatabase>();

        // Iterate through all predefined roles
        foreach (var role in InitialRoles.All)
        {
            // Skip creation if the role already exists in the system
            if (await roleManager.RoleExistsAsync(role.Name!))
            {
                continue;
            }

            // Attempt to create the role
            var resultCreate = await roleManager.CreateAsync(role);
            if (!resultCreate.Succeeded)
            {
                // If creation fails, throw an exception with detailed error messages
                throw new Exception($"Failed to create role {role.Name}: {string.Join(", ", resultCreate.Errors.Select(e => e.Description))}");
            }

            // If role creation succeeded, attach all predefined claims to this role
            if (InitialRoleClaim.All.Any())
            {
                foreach (var claim in InitialRoleClaim.All)
                {
                    var resultAdd = await roleManager.AddClaimAsync(role, new Claim(claim.ClaimType!, claim.ClaimValue!));
                    if (!resultAdd.Succeeded)
                    {
                        // Fail fast if adding a claim fails, with detailed error info
                        throw new Exception($"Failed to add claim {claim.ClaimType} to role {role.Name}: {string.Join(", ", resultAdd.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }

        // Persist any changes to the database (only executes if there are changes)
        var resultSaveChanges = await dbContext.SaveChangesAsync();
    }
}
