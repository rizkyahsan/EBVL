using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Pertamina.Extensions.Identity;
using Pertamina.Extensions.Identity.Statics;
using Pertamina.Services.PersonalRoles;
using Pertamina.Services.PositionRoles;
using Pertamina.Services.UserPositions;

namespace EBVL.BackEnd.Infrastructure.Authentication;

public sealed class CustomJwtBearerEvents(
    IUserPositionsService userPositionsService,
    IPositionRolesService positionRolesService,
    IPersonalRolesService personalRolesService,
    ILogger<CustomJwtBearerEvents> logger) : JwtBearerEvents
{
    private const string PositionIdHeader = "PositionId";

    public override Task AuthenticationFailed(AuthenticationFailedContext context)
    {
        logger.LogError(context.Exception, "JWT Bearer Authentication failed. {ErrorMessage}", context.Exception.Message);
        return Task.CompletedTask;
    }

    public override async Task TokenValidated(TokenValidatedContext context)
    {
        if (context.Principal?.Identity is not ClaimsIdentity identity)
        {
            context.Fail("The access token has no claims identity.");
            return;
        }

        var userId = context.Principal.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            context.Fail("The access token has no user identifier.");
            return;
        }

        var positionId = context.HttpContext.Request.Headers[PositionIdHeader].ToString();
        if (!string.IsNullOrWhiteSpace(positionId))
        {
            var positions = await userPositionsService.GetUserPositions(userId);
            var selectedPosition = positions.Positions.SingleOrDefault(position => position.Id == positionId);
            if (selectedPosition is null)
            {
                context.Fail("The selected position does not belong to the authenticated user.");
                return;
            }

            AddClaim(identity, ClaimTypeFor.PositionId, selectedPosition.Id);
            AddClaim(identity, ClaimTypeFor.PositionName, selectedPosition.Name);
            var positionRoles = await positionRolesService.GetPositionRolesAsync(selectedPosition.Id);
            foreach (var role in positionRoles.Roles)
            {
                AddRole(identity, role.Name);
                AddPermissions(identity, role.Permissions);
            }
        }

        var personalRoles = await personalRolesService.GetPersonalRolesAsync(userId);
        foreach (var role in personalRoles.Roles)
        {
            AddRole(identity, role.Name);
            AddPermissions(identity, role.Permissions);
        }
    }

    private static void AddRole(ClaimsIdentity identity, string role)
    {
        AddClaim(identity, ClaimTypes.Role, role);
        AddClaim(identity, "role", role);
    }

    private static void AddPermissions(ClaimsIdentity identity, IEnumerable<string> permissions)
    {
        foreach (var permission in permissions)
        {
            AddClaim(identity, ClaimTypeFor.Permission, permission);
            AddClaim(identity, "scope", permission);
        }
    }

    private static void AddClaim(ClaimsIdentity identity, string type, string value)
    {
        if (!identity.HasClaim(type, value))
        {
            identity.AddClaim(new Claim(type, value));
        }
    }
}
