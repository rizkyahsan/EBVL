using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EBVL.FrontEnd.Infrastructure.Authentication.Statics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.HttpResults;
using Pertamina.Common.Statics;
using Pertamina.Extensions.Identity;
using Pertamina.Extensions.Identity.Statics;
using Pertamina.Services.IdAMan.Statics;
using Pertamina.Services.PersonalRoles;
using Pertamina.Services.PositionRoles;
using Pertamina.Services.UserPositions;

namespace EBVL.FrontEnd.Infrastructure.Authentication;

public static class AuthenticationHandlers
{
    public static ChallengeHttpResult LoginHandler(string pathBase, string? returnUrl)
    {
        var authenticationProperties = GenerateAuthenticationProperties(pathBase, returnUrl);

        return TypedResults.Challenge(authenticationProperties, [OpenIdConnectDefaults.AuthenticationScheme]);
    }

    public static SignOutHttpResult LogoutHandler(string pathBase, string? returnUrl, IHttpContextAccessor httpContextAccessor)
    {
        if (httpContextAccessor.HttpContext is null)
        {
            return TypedResults.SignOut();
        }

        if (httpContextAccessor.HttpContext.User.Identity is not ClaimsIdentity currentIdentity)
        {
            return TypedResults.SignOut();
        }

        var authenticationProperties = GenerateAuthenticationProperties(pathBase, returnUrl);

        if (currentIdentity.AuthenticationType is AuthenticationTypeFor.LocalAuthentication)
        {
            var localAuthenticationSchemes = new[]
            {
                CookieAuthenticationDefaults.AuthenticationScheme
            };

            return TypedResults.SignOut(authenticationProperties, localAuthenticationSchemes);
        }

        var authenticationSchemes = new[]
        {
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme
        };

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

    public static async Task<RedirectHttpResult> LocalLoginHandler(string sessionId, string? returnUrl, IHttpContextAccessor httpContextAccessor)
    {
        if (httpContextAccessor.HttpContext is null)
        {
            return TypedResults.Redirect(RouteFor.Landing);
        }

        #region Verification
        var httpContext = httpContextAccessor.HttpContext;
        var userToken = UserTokenStore.GetSession(new Guid(sessionId));
        if (userToken is null)
        {
            return TypedResults.Redirect(RouteFor.Landing);
        }

        var currentIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var currentUserAgent = httpContext.Request.Headers.UserAgent.ToString();
        UserTokenStore.RemoveSession(new Guid(sessionId));
        if (userToken.IpAddress != currentIp)
        {
            return TypedResults.Redirect(RouteFor.Landing);
        }

        if (userToken.UserAgent != currentUserAgent)
        {
            return TypedResults.Redirect(RouteFor.Landing);
        }
        #endregion

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(userToken.UserToken);
        var authenticationTime = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
        var displayName = token.Claims.FirstOrDefault(x => x.Type is JwtRegisteredClaimNames.Name)?.Value ?? "Local User";
        var email = token.Claims.FirstOrDefault(x => x.Type is JwtRegisteredClaimNames.Email)?.Value ?? "No Email Address";
        var role = token.Claims.FirstOrDefault(x => x.Type is "role")?.Value ?? "No Role Assign";
        var lender = token.Claims.FirstOrDefault(x => x.Type is ClaimTypeFor.CompanyName)?.Value ?? "No Lender Assign";
        var photoUrl = "img/local-user.png";

        var identity = new ClaimsIdentity(token.Claims, AuthenticationTypeFor.LocalAuthentication);
        identity.AddClaim(new Claim(JwtRegisteredClaimNames.AuthTime, authenticationTime));
        identity.AddClaim(new Claim(ClaimTypeFor.DisplayName, displayName));
        identity.AddClaim(new Claim(ClaimTypes.Email, email));
        identity.AddClaim(new Claim(ClaimTypeFor.PhotoUrl, photoUrl));
        identity.AddClaim(new Claim(ClaimTypes.Role, role));
        identity.AddClaim(new Claim(ClaimTypeFor.CompanyName, lender));

        var authenticationProperties = await GetAuthenticationProperties(httpContextAccessor.HttpContext);

        var tokens = new List<AuthenticationToken>
        {
            new() { Name = TokenNameFor.TokenType, Value = "Bearer" },
            new() { Name = TokenNameFor.AccessToken, Value = userToken.UserToken },
            new() { Name = TokenNameFor.ExpiresAt, Value = DateTimeOffset.Now.AddDays(1).ToString("o") }
        };

        authenticationProperties.StoreTokens(tokens);

        var claimsPrincipal = new ClaimsPrincipal(identity);

        await httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authenticationProperties);

        //if (string.IsNullOrWhiteSpace(returnUrl))
        //{
        //    return TypedResults.Redirect("/");
        //}

        returnUrl = "/MyProjects";
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
