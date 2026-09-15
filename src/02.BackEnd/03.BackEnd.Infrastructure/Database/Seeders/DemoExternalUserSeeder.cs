using EBVL.BackEnd.Infrastructure.LocalIdentity.Models;
using EBVL.Shared.Statics;
using Microsoft.AspNetCore.Identity;

namespace EBVL.BackEnd.Infrastructure.Database.Seeders;

public sealed class DemoExternalUserSeeder(
    IDatabaseService databaseService,
    UserManager<AspNetCoreUser> userManager)
{
    private const string EmailEnvironmentVariable = "EBVL_DEMO_VENDOR_EMAIL";
    private const string PasswordEnvironmentVariable = "EBVL_DEMO_VENDOR_PASSWORD";

    public async Task SeedDemoExternalUser()
    {
        var email = Environment.GetEnvironmentVariable(EmailEnvironmentVariable);
        var password = Environment.GetEnvironmentVariable(PasswordEnvironmentVariable);
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var identityUser = await userManager.FindByEmailAsync(email);
        if (identityUser is null)
        {
            identityUser = new AspNetCoreUser
            {
                Id = Guid.CreateVersion7(),
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                IsDeactivated = false
            };

            var createResult = await userManager.CreateAsync(identityUser, password);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(string.Join(", ", createResult.Errors.Select(error => error.Description)));
            }
        }
        else if (!identityUser.EmailConfirmed || identityUser.IsDeactivated)
        {
            identityUser.EmailConfirmed = true;
            identityUser.IsDeactivated = false;
            var updateResult = await userManager.UpdateAsync(identityUser);
            if (!updateResult.Succeeded)
            {
                throw new InvalidOperationException(string.Join(", ", updateResult.Errors.Select(error => error.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(identityUser, RoleNameFor.Lender))
        {
            var roleResult = await userManager.AddToRoleAsync(identityUser, RoleNameFor.Lender);
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(string.Join(", ", roleResult.Errors.Select(error => error.Description)));
            }
        }

        if (await databaseService.Users.AnyAsync(user => !user.IsDeleted && user.IdentityUserId == identityUser.Id))
        {
            return;
        }

        var country = await databaseService.Countries.SingleOrDefaultAsync(item => !item.IsDeleted && item.Code == "ID");
        if (country is null)
        {
            country = InitialCountries.Indonesia;
            _ = await databaseService.Countries.AddAsync(country);
        }

        var lender = await databaseService.Lenders.SingleOrDefaultAsync(item => !item.IsDeleted && item.EmailAddress == email);
        if (lender is null)
        {
            lender = new Lender
            {
                Id = Guid.CreateVersion7(),
                Name = "Demo Vendor",
                Address = "Jakarta",
                CountryId = country.Id,
                PhoneNumber = "0000000000",
                EmailAddress = email,
                Website = string.Empty
            };
            _ = await databaseService.Lenders.AddAsync(lender);
        }

        _ = await databaseService.Users.AddAsync(new User
        {
            Id = Guid.CreateVersion7(),
            IdentityUserId = identityUser.Id,
            LenderId = lender.Id,
            Username = email,
            DisplayName = "Demo Vendor",
            EmailAddress = email,
            OtpSecret = null,
            OtpUrl = null,
            IsVerified = true,
            IsPicLender = true
        });
        _ = await databaseService.SaveAsync(nameof(SeedDemoExternalUser));
    }
}
