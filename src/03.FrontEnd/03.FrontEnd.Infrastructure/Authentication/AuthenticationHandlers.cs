using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Pertamina.Common.Statics;
using Pertamina.Extensions.Identity;
using Pertamina.Extensions.Identity.Statics;
using Pertamina.Services.IdAMan.Statics;
using Pertamina.Services.PersonalRoles;
using Pertamina.Services.PositionRoles;
using Pertamina.Services.UserPositions;
using EBVL.FrontEnd.Infrastructure.Authentication.Statics;
using EBVL.FrontEnd.Services.BackEndApi;
using EBVL.Shared.Dto.Modules.Administration.VendorRegistrations.AuthenticateVendor;
using RestSharp;

namespace EBVL.FrontEnd.Infrastructure.Authentication;

public static class AuthenticationHandlers
{
    public static async Task<IResult> LocalLoginHandler(
        [FromForm] string emailAddress,
        [FromForm] string password,
        [FromForm] bool? rememberMe,
        [FromForm] string? returnUrl,
        HttpContext httpContext,
        IBackEndApiService backEndApiService,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new AuthenticateVendorRequest
            {
                EmailAddress = emailAddress,
                Password = password
            };
            var restRequest = new RestRequest(AuthenticateVendorRoute.ResourceUri, Method.Post).AddJsonBody(request);
            var vendor = await backEndApiService.SendAnonymousRequestAsync<AuthenticateVendorResponse>(restRequest, cancellationToken);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, vendor.VendorAccountId.ToString()),
                new Claim(ClaimTypes.Name, vendor.CompanyName),
                new Claim(ClaimTypes.Email, vendor.EmailAddress),
                new Claim(ClaimTypes.Role, "Vendor"),
                new Claim(ClaimTypes.AuthenticationMethod, "LocalAccount"),
                new Claim("VendorRegistrationId", vendor.VendorRegistrationId.ToString()),
                new Claim("SapVendorNumber", vendor.SapVendorNumber),
                new Claim("VendorRegistrationStatus", vendor.Status.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var properties = new AuthenticationProperties
            {
                IsPersistent = rememberMe is true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(rememberMe is true ? 12 : 2)
            };
            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                properties);

            var redirect = string.IsNullOrWhiteSpace(returnUrl) || !Uri.IsWellFormedUriString(returnUrl, UriKind.Relative)
                ? "/Vendor/Registration/Summary"
                : returnUrl;
            return Results.LocalRedirect(redirect);
        }
        catch
        {
            var encodedEmail = Uri.EscapeDataString(emailAddress);
            return Results.LocalRedirect($"/Vendor/Login?error=invalid&email={encodedEmail}");
        }
    }

    public static ChallengeHttpResult LoginHandler(string pathBase, string? returnUrl)
    {
        var authenticationProperties = GenerateAuthenticationProperties(pathBase, returnUrl);

        return TypedResults.Challenge(authenticationProperties, [OpenIdConnectDefaults.AuthenticationScheme]);
    }

    public static SignOutHttpResult LogoutHandler(string pathBase, string? returnUrl, HttpContext httpContext)
    {
        var authenticationProperties = GenerateAuthenticationProperties(pathBase, returnUrl);

        var isLocalAccount = httpContext.User.FindFirstValue(ClaimTypes.AuthenticationMethod) == "LocalAccount";
        var authenticationSchemes = isLocalAccount
            ? [CookieAuthenticationDefaults.AuthenticationScheme]
            : new[] { CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme };

        return TypedResults.SignOut(authenticationProperties, authenticationSchemes);
    }

    public static async Task<RedirectHttpResult> SwitchPositionHandler(string positionId, string? returnUrl, IHttpContextAccessor httpContextAccessor, IUserPositionsService userPositionsService, IPositionRolesService positionRolesService, IPersonalRolesService personalRolesService)
    {
        if (httpContextAccessor.HttpContext is null)
        {
            return TypedResults.Redirect(RouteFor.Landing);
        }

        if (httpContextAccessor.HttpContext.User.Identity is not ClaimsIdentity currentIdentity)
        {
            return TypedResults.Redirect(RouteFor.Landing);
        }

        var userId = httpContextAccessor.HttpContext.User.GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return TypedResults.Redirect(RouteFor.Landing);
        }

        var existingClaims = currentIdentity.Claims.Where(x => x.Type
            is not ClaimTypes.Role
            and not ClaimTypeFor.Permission
            and not ClaimTypeFor.PositionId
            and not ClaimTypeFor.PositionName);

        var identity = new ClaimsIdentity(existingClaims, currentIdentity.AuthenticationType);

        await ProcessPositionRoles(userPositionsService, positionRolesService, identity, positionId, userId);
        await ProcessPersonalRoles(personalRolesService, identity, userId);

        var authenticationProperties = await GetAuthenticationProperties(httpContextAccessor.HttpContext);
        var claimsPrincipal = new ClaimsPrincipal(identity);

        await httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authenticationProperties);

        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            return TypedResults.Redirect("");
        }

        return TypedResults.Redirect(returnUrl);
    }

    private static async Task ProcessPositionRoles(IUserPositionsService userPositionsService, IPositionRolesService positionRolesService, ClaimsIdentity identity, string positionId, string userId)
    {
        var getUserPositionsResult = await userPositionsService.GetUserPositions(userId);
        var selectedPosition = getUserPositionsResult.Positions.FirstOrDefault(x => x.Id == positionId)
            ?? throw new InvalidOperationException($"{DisplayTextFor.User} with {DisplayTextFor.Id} [{userId}] does not have {DisplayTextFor.Position} with {DisplayTextFor.Id} [{positionId}].");

        identity.AddClaim(new Claim(ClaimTypeFor.PositionId, selectedPosition.Id));
        identity.AddClaim(new Claim(ClaimTypeFor.PositionName, selectedPosition.Name));

        var positionRolesResult = await positionRolesService.GetPositionRolesAsync(selectedPosition.Id);

        foreach (var role in positionRolesResult.Roles)
        {
            if (!identity.Claims.Any(x => x.Type == ClaimTypes.Role && x.Value == role.Name))
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role.Name));
            }

            foreach (var permission in role.Permissions.Where(permission => !identity.Claims.Any(x => x.Type == ClaimTypeFor.Permission && x.Value == permission)))
            {
                identity.AddClaim(new Claim(ClaimTypeFor.Permission, permission));
            }
        }
    }

    private static async Task ProcessPersonalRoles(IPersonalRolesService personalRolesService, ClaimsIdentity identity, string userId)
    {
        var personalRolesResult = await personalRolesService.GetPersonalRolesAsync(userId);

        foreach (var role in personalRolesResult.Roles)
        {
            if (!identity.Claims.Any(x => x.Type == ClaimTypes.Role && x.Value == role.Name))
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role.Name));
            }

            foreach (var permission in role.Permissions.Where(permission => !identity.Claims.Any(x => x.Type == ClaimTypeFor.Permission && x.Value == permission)))
            {
                identity.AddClaim(new Claim(ClaimTypeFor.Permission, permission));
            }
        }

        foreach (var customParameter in personalRolesResult.CustomParameters)
        {
            var customParameterClaimType = $"{ClaimTypeFor.CustomParameter}{DelimiterFor.CustomParameter}{customParameter.Key}";

            if (!identity.Claims.Any(x => x.Type == customParameterClaimType && x.Value == customParameter.Value))
            {
                identity.AddClaim(new Claim(customParameterClaimType, customParameter.Value));
            }
        }
    }

    private static async Task<AuthenticationProperties> GetAuthenticationProperties(HttpContext httpContext)
    {
        var tokens = new List<AuthenticationToken>();

        var tokenType = await httpContext.GetTokenAsync(TokenNameFor.TokenType);

        if (!string.IsNullOrWhiteSpace(tokenType))
        {
            tokens.Add(new AuthenticationToken { Name = TokenNameFor.TokenType, Value = tokenType });
        }

        var accessToken = await httpContext.GetTokenAsync(TokenNameFor.AccessToken);

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            tokens.Add(new AuthenticationToken { Name = TokenNameFor.AccessToken, Value = accessToken });
        }

        var refreshToken = await httpContext.GetTokenAsync(TokenNameFor.RefreshToken);

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            tokens.Add(new AuthenticationToken { Name = TokenNameFor.RefreshToken, Value = refreshToken });
        }

        var expiresAt = await httpContext.GetTokenAsync(TokenNameFor.ExpiresAt);

        if (!string.IsNullOrWhiteSpace(expiresAt))
        {
            tokens.Add(new AuthenticationToken { Name = TokenNameFor.ExpiresAt, Value = expiresAt });
        }

        var authenticationProperties = new AuthenticationProperties();
        authenticationProperties.StoreTokens(tokens);

        return authenticationProperties;
    }

    private static AuthenticationProperties GenerateAuthenticationProperties(string pathBase, string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            return new AuthenticationProperties
            {
                RedirectUri = pathBase
            };
        }

        if (!Uri.IsWellFormedUriString(returnUrl, UriKind.Relative))
        {
            var redirectUri = new Uri(returnUrl, UriKind.Absolute).PathAndQuery;

            return new AuthenticationProperties
            {
                RedirectUri = redirectUri
            };
        }

        if (returnUrl[0] != '/')
        {
            var redirectUri = $"{pathBase}{returnUrl}";

            return new AuthenticationProperties
            {
                RedirectUri = redirectUri
            };
        }

        return new AuthenticationProperties
        {
            RedirectUri = returnUrl
        };
    }
}
