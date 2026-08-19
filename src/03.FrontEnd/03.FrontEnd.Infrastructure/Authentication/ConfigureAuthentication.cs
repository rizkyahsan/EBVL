using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Pertamina.Extensions.Identity.Statics;
using Pertamina.Services.IdAMan;
using Pertamina.Services.PersonalRoles;
using Pertamina.Services.PositionRoles;
using Pertamina.Services.UserPositions;
using EBVL.FrontEnd.Infrastructure.Authentication.Statics;

namespace EBVL.FrontEnd.Infrastructure.Authentication;

public static class ConfigureAuthentication
{
    public static IServiceCollection AddAuthenticationService(this IServiceCollection services, IdAManOptions idAManOptions)
    {
        _ = services.AddScoped<CustomOpenIdConnectEvents>();
        _ = services.AddScoped<CustomCookieAuthenticationEvents>();

        _ = services
        .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.SignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
            options.SaveTokens = true;
            options.MapInboundClaims = false;

            options.Authority = idAManOptions.Authentication.AuthorityUrl;
            options.ClientId = idAManOptions.ClientId;
            options.ClientSecret = idAManOptions.ClientSecret;
            options.CallbackPath = "/signin-oidc";
            options.CorrelationCookie.Path = "/signin-oidc";
            options.NonceCookie.Path = "/signin-oidc";
            options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
            options.EventsType = typeof(CustomOpenIdConnectEvents);
            options.Scope.Add(OpenIdConnectScope.Email);
            options.Scope.Add(OpenIdConnectScope.OfflineAccess);

            options.TokenValidationParameters = new()
            {
                NameClaimType = ClaimTypeFor.DisplayName
            };
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.LoginPath = new PathString($"/{RouteFor.Landing}");
            options.LogoutPath = new PathString($"/{RouteFor.Landing}");
            options.AccessDeniedPath = new PathString($"/{RouteFor.AccessDenied}");
            options.EventsType = typeof(CustomCookieAuthenticationEvents);
        });

        return services;
    }

    public static WebApplication MapAuthenticationEndpoints(this WebApplication app, string pathBase)
    {
        var finalPathBase = "/";

        if (!string.IsNullOrWhiteSpace(pathBase))
        {
            finalPathBase = pathBase;
        }

        var routeGroupBuilder = app.MapGroup(PrefixFor.Authentication);

        _ = routeGroupBuilder
            .MapGet(PatternFor.Login, (string? returnUrl) => AuthenticationHandlers.LoginHandler(finalPathBase, returnUrl))
            .AllowAnonymous();

        _ = routeGroupBuilder
            .MapGet(PatternFor.Logout, (string? returnUrl, IHttpContextAccessor httpContextAccessor) => AuthenticationHandlers.LogoutHandler(finalPathBase, returnUrl, httpContextAccessor))
            .RequireAuthorization();

        _ = routeGroupBuilder
            .MapGet(PatternFor.SwitchPosition, ([FromRoute] string positionId, string? returnUrl, IHttpContextAccessor httpContextAccessor, IUserPositionsService userPositionsService, IPositionRolesService positionRolesService, IPersonalRolesService personalRolesService) => AuthenticationHandlers.SwitchPositionHandler(positionId, returnUrl, httpContextAccessor, userPositionsService, positionRolesService, personalRolesService))
            .RequireAuthorization();

        _ = routeGroupBuilder
            .MapGet(PatternFor.LocalLoginHandler, ([FromRoute] string sessionId, string? returnUrl, IHttpContextAccessor httpContextAccessor) => AuthenticationHandlers.LocalLoginHandler(sessionId, returnUrl, httpContextAccessor))
            .AllowAnonymous();

        return app;
    }
}
