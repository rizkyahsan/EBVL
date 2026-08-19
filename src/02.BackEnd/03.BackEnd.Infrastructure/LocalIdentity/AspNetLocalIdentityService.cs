using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EBVL.BackEnd.Infrastructure.LocalIdentity.Extensions;
using EBVL.BackEnd.Infrastructure.LocalIdentity.Models;
using EBVL.BackEnd.Services.LocalIdentity;
using EBVL.BackEnd.Services.LocalIdentity.Model;
using EBVL.Shared.Statics;
using EBVL.Shared.Statics.Common;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Pertamina.Extensions.Identity.Statics;
using Pertamina.Services.IdAMan;

namespace EBVL.BackEnd.Infrastructure.LocalIdentity;

public class AspNetLocalIdentityService(UserManager<AspNetCoreUser> userManager,
    RoleManager<AspNetCoreRole> roleManager,
    IPasswordHasher<AspNetCoreUser> passwordHasher,
    IOptions<IdAManOptions> idAManOptions,
    IOptions<LocalIdentityOptions> localIdentityOptions,
    IDatabaseService databaseService) : ILocalIdentityService
{
    public async Task<Guid> CreateUserAsync(string username, string email, string password)
    {
        var aspNetCoreUser = new AspNetCoreUser
        {
            Id = Guid.CreateVersion7(),
            UserName = username,
            Email = email,
            IsDeactivated = false
        };

        var identityResult = await userManager.CreateAsync(aspNetCoreUser, password);

        if (!identityResult.Succeeded)
        {
            throw new InvalidOperationException(identityResult.GetErrorSummary());
        }

        _ = await userManager.AddToRoleAsync(aspNetCoreUser, RoleNameFor.Lender);

        return aspNetCoreUser.Id;
    }

    public async Task<bool> CheckAccessAsync(Guid id, string token)
    {
        var user = await databaseService.Users
            .Where(x => !x.IsDeleted && x.Id == id)
            .SingleOrDefaultAsync()
            ?? throw new InvalidOperationException("No Access, please contact Administrator!");

        if (user.AccessTokenHash is null || user.AccessTokenExpiredAt is null)
        {
            throw new InvalidOperationException("No Access, please contact Administrator!");
        }

        var now = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, TimezoneFor.WibTimeZone);
        if (user.AccessTokenExpiredAt <= now)
        {
            throw new InvalidOperationException("Expired Access, please contact Administrator!");
        }

        var tokenPayload = $"{user.Id}.{token}";

        var hashedIncomingToken = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(tokenPayload))
        );

        var isValid = CryptographicOperations.FixedTimeEquals(
            Convert.FromHexString(hashedIncomingToken),
            Convert.FromHexString(user.AccessTokenHash)
        );

        if (!isValid)
        {
            user.AccessTokenHash = null;
            user.AccessTokenExpiredAt = null;

            _ = await databaseService.SaveAsync("Update User");

            throw new InvalidOperationException("Invalid Access, please contact Administrator!");
        }

        return isValid;
    }

    public async Task<Guid> VerifyUserAsync(Guid id)
    {
        var user = await userManager.Users
            .SingleAsync(m => m.Id == id).ConfigureAwait(false);

        var now = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, TimezoneFor.WibTimeZone);
        user.EmailConfirmed = true;
        user.Modified = now;
        user.ModifiedBy = "EBVLSystem";

        var identityResult = await userManager.UpdateAsync(user);

        if (!identityResult.Succeeded)
        {
            throw new InvalidOperationException(identityResult.GetErrorSummary());
        }

        return user.Id;
    }

    public async Task<LoginResult> VerifyUserPasswordAsync(string username, string password)
    {
        var user = await userManager.FindByNameAsync(username);

        if (user is null)
        {
            return LoginResult.Failed("User not found");
        }

        var passwordIsCorrect = await userManager.CheckPasswordAsync(user, password);

        if (!passwordIsCorrect)
        {
            return LoginResult.Failed("Incorrect password");
        }

        return LoginResult.Success(user.Id);
    }

    public async Task<Guid> UpdatePasswordAsync(Guid id, string password)
    {
        var user = await userManager.Users
            .SingleAsync(m => m.Id == id).ConfigureAwait(false);

        if (!string.IsNullOrWhiteSpace(password))
        {
            var now = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, TimezoneFor.WibTimeZone);
            user.PasswordHash = passwordHasher.HashPassword(user, password);
            user.LastPasswordModified = now;
        }

        var identityResult = await userManager.UpdateAsync(user);

        if (!identityResult.Succeeded)
        {
            throw new InvalidOperationException(identityResult.GetErrorSummary());
        }

        return user.Id;
    }

    public async Task<Guid> DeactiveUserAsync(Guid id)
    {
        var user = await userManager.Users
            .SingleAsync(m => m.Id == id).ConfigureAwait(false);

        var now = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, TimezoneFor.WibTimeZone);
        user.IsDeactivated = true;
        user.Modified = now;
        user.ModifiedBy = "EBVLSystem";

        var identityResult = await userManager.UpdateAsync(user);

        if (!identityResult.Succeeded)
        {
            throw new InvalidOperationException(identityResult.GetErrorSummary());
        }

        return user.Id;
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await userManager.Users
            .SingleAsync(m => m.Id == id).ConfigureAwait(false);

        var identityResult = await userManager.DeleteAsync(user);

        if (!identityResult.Succeeded)
        {
            throw new InvalidOperationException(identityResult.GetErrorSummary());
        }
    }

    public async Task<LoginResult> LoginAsync(string username)
    {
        var userIdentity = await userManager.FindByNameAsync(username);

        if (userIdentity is null)
        {
            return LoginResult.Failed("User not found");
        }

        var user = await databaseService.Users
            .AsNoTracking()
            .Include(m => m.Lender)
            .Where(x => x.IdentityUserId == userIdentity.Id)
            .SingleOrDefaultAsync();

        if (user is null)
        {
            return LoginResult.Failed("Lender for this user not found");
        }

        var claims = await userManager.GetClaimsAsync(userIdentity);
        claims.Add(new Claim(JwtClaimTypes.Subject, userIdentity.Id.ToString()));
        claims.Add(new Claim(JwtClaimTypes.Name, user.DisplayName));
        claims.Add(new Claim(JwtClaimTypes.PreferredUserName, username));
        claims.Add(new Claim(ClaimTypes.Email, userIdentity.Email!));
        claims.Add(new Claim(ClaimTypeFor.CompanyName, user.Lender.Name));

        var listUserRole = await userManager.GetRolesAsync(userIdentity);

        if (listUserRole.Count < 1)
        {
            return LoginResult.Failed("User not have role assign");
        }

        foreach (var userRole in listUserRole)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole));

            var role = await roleManager.FindByNameAsync(userRole);
            var roleClaims = await roleManager.GetClaimsAsync(role!);

            foreach (var roleClaim in roleClaims)
            {
                claims.Add(new Claim("permission", roleClaim.Value));
                claims.Add(new Claim("scope", roleClaim.Value));
            }
        }

        return LoginResult.Success(claims);
    }

    public string GenerateToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(localIdentityOptions.Value.Secret))
        {
            KeyId = localIdentityOptions.Value.Key
        };

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = localIdentityOptions.Value.Issuer,
            Audience = $"api://{idAManOptions.Value.ObjectId}",
            Expires = DateTime.Now.AddHours(1),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
