using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Pertamina.Extensions.Identity;
using Pertamina.Extensions.Identity.Statics;
using Pertamina.Services.IdAMan.Statics;
using Pertamina.Services.PersonalRoles;
using Pertamina.Services.PositionRoles;
using Pertamina.Services.UserPositions;
using Pertamina.Services.UserProfile;

namespace EBVL.FrontEnd.Infrastructure.Authentication;

public sealed class CustomOpenIdConnectEvents(
    IUserProfileService userProfileService,
    IPositionRolesService positionRolesService,
    IPersonalRolesService personalRolesService,
    IUserPositionsService userPositionsService)
    : OpenIdConnectEvents
{
    private const string SignInCallbackPath = "/signin-oidc";

    public override Task RedirectToIdentityProvider(RedirectContext context)
    {
        context.ProtocolMessage.RedirectUri = $"{context.Request.Scheme}://{context.Request.Host}{SignInCallbackPath}";

        return Task.CompletedTask;
    }

    public override async Task TokenResponseReceived(TokenResponseReceivedContext context)
    {
        var principal = context.Principal;

        if (principal is null)
        {
            return;
        }

        if (principal.Identity is not ClaimsIdentity identity)
        {
            return;
        }

        var userId = principal.GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new InvalidOperationException($"userId is NULL.");
        }

        var email = principal.GetUsername();

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidOperationException($"email is NULL.");
        }

        await ProcessUserProfile(identity, email);
        await ProcessUserPositions(identity, userId);
        await ProcessPersonalRoles(identity, userId);
    }

    private async Task ProcessUserProfile(ClaimsIdentity identity, string email)
    {
        var userProfileResult = await userProfileService.GetUserProfileAsync(email);

        identity.AddClaim(new Claim(ClaimTypes.Email, userProfileResult.EmailAddress));
        identity.AddClaim(new Claim(ClaimTypeFor.CompanyName, userProfileResult.CompanyName));

        if (string.IsNullOrWhiteSpace(userProfileResult.Kbo))
        {
            identity.AddClaim(new Claim(ClaimTypeFor.Kbo, string.Empty));
        }
        else
        {
            identity.AddClaim(new Claim(ClaimTypeFor.Kbo, userProfileResult.Kbo));
        }

        if (string.IsNullOrWhiteSpace(userProfileResult.PhoneNumber))
        {
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.PhoneNumber, string.Empty));
        }
        else
        {
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.PhoneNumber, userProfileResult.PhoneNumber));
        }

        if (string.IsNullOrWhiteSpace(userProfileResult.PhotoUrl))
        {
            identity.AddClaim(new Claim(ClaimTypeFor.PhotoUrl, "img/user.png"));
        }
        else
        {
            identity.AddClaim(new Claim(ClaimTypeFor.PhotoUrl, userProfileResult.PhotoUrl));
        }
    }

    private async Task ProcessUserPositions(ClaimsIdentity identity, string userId)
    {
        var userPositionsResult = await userPositionsService.GetUserPositions(userId);

        var defaultPosition = userPositionsResult.Positions.FirstOrDefault(x => x.PersonaType.Equals(PersonaTypeFor.Permanent, StringComparison.OrdinalIgnoreCase));
        defaultPosition ??= userPositionsResult.Positions.FirstOrDefault(x => x.PersonaType.Equals(PersonaTypeFor.Temporary, StringComparison.OrdinalIgnoreCase));
        defaultPosition ??= userPositionsResult.Positions.FirstOrDefault(x => x.PersonaType.Equals(PersonaTypeFor.Functional, StringComparison.OrdinalIgnoreCase));
        defaultPosition ??= userPositionsResult.Positions.FirstOrDefault(x => x.PersonaType.Equals(PersonaTypeFor.AdHoc, StringComparison.OrdinalIgnoreCase));

        if (defaultPosition is null)
        {
            return;
        }

        identity.AddClaim(new Claim(ClaimTypeFor.PositionId, defaultPosition.Id));
        identity.AddClaim(new Claim(ClaimTypeFor.PositionName, defaultPosition.Name));

        var positionRolesResult = await positionRolesService.GetPositionRolesAsync(defaultPosition.Id);

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

        foreach (var customParameter in positionRolesResult.CustomParameters)
        {
            var customParameterClaimType = $"{ClaimTypeFor.CustomParameter}{DelimiterFor.CustomParameter}{customParameter.Key}";

            if (!identity.Claims.Any(x => x.Type == customParameterClaimType && x.Value == customParameter.Value))
            {
                identity.AddClaim(new Claim(customParameterClaimType, customParameter.Value));
            }
        }
    }

    private async Task ProcessPersonalRoles(ClaimsIdentity identity, string userId)
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
}
