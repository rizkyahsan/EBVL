using System.Globalization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Pertamina.Services.IdAMan;
using EBVL.FrontEnd.Infrastructure.Authentication.Statics;

namespace EBVL.FrontEnd.Infrastructure.Authentication;

public sealed class CustomCookieAuthenticationEvents(
    IdAManServiceProvider idAManServiceProvider,
    ILogger<CustomCookieAuthenticationEvents> logger)
    : CookieAuthenticationEvents
{
    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        var authenticationProperties = context.Properties;
        var authenticationType = context.Principal!.Identity!.AuthenticationType;

        if (authenticationType is AuthenticationTypeFor.LocalAuthentication)
        {
            return;
        }

        var expiresAt = authenticationProperties.GetTokenValue(TokenNameFor.ExpiresAt);

        if (!DateTimeOffset.TryParse(expiresAt, CultureInfo.InvariantCulture, out var expires))
        {
            throw new InvalidOperationException($"Failed to parse {expiresAt} into {nameof(DateTimeOffset)} instance.");
        }

        var localNow = DateTimeOffset.Now.LocalDateTime;
        var localExpires = expires.LocalDateTime;
        var isExpired = localNow >= localExpires;

        if (!isExpired)
        {
            return;
        }

        try
        {
            var refreshToken = authenticationProperties.GetTokenValue(TokenNameFor.RefreshToken);
            var idAManRefreshToken = await idAManServiceProvider.GetRefreshTokenAsync(refreshToken!, context.HttpContext.RequestAborted);

            _ = authenticationProperties.UpdateTokenValue(TokenNameFor.AccessToken, idAManRefreshToken.AccessToken);
            _ = authenticationProperties.UpdateTokenValue(TokenNameFor.RefreshToken, idAManRefreshToken.RefreshToken);
            _ = authenticationProperties.UpdateTokenValue(TokenNameFor.ExpiresAt, DateTime.Now.AddSeconds(idAManRefreshToken.ExpiresIn).ToString("o"));

            context.ShouldRenew = true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while refreshing the access token.");

            await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
