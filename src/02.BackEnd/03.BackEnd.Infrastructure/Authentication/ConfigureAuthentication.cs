using System.Security.Claims;
using System.Text;
using EBVL.BackEnd.Infrastructure.LocalIdentity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Pertamina.Services.IdAMan;

namespace EBVL.BackEnd.Infrastructure.Authentication;

public static class ConfigureAuthentication
{
    public static IServiceCollection AddAuthenticationService(
        this IServiceCollection services,
        IdAManOptions idAManOptions,
        LocalIdentityOptions localIdentityOptions)
    {
        _ = services.AddScoped<CustomJwtBearerEvents>();

        // Setup for dual Login Method (IdAMan & AspNetIdentity)
        _ = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "MultiAuth";
            options.DefaultChallengeScheme = "MultiAuth";
        })
        .AddPolicyScheme("MultiAuth", "MultiAuth", options =>
        {
            options.ForwardDefaultSelector = context =>
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();

                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    var token = authHeader["Bearer ".Length..].Trim();

                    var jwt = new JsonWebToken(token);

                    if (jwt.Issuer == idAManOptions.Authentication.AuthorityUrl)
                    {
                        return "IdAMan";
                    }

                    if (jwt.Issuer == localIdentityOptions.Issuer)
                    {
                        return "Local";
                    }
                }

                return "IdAMan"; // default to IdAMan
            };
        })
        .AddJwtBearer("IdAMan", options =>
        {
            options.Authority = idAManOptions.Authentication.AuthorityUrl;
            options.Audience = $"api://{idAManOptions.ObjectId}";
            options.MapInboundClaims = false;

            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = false,
                ClockSkew = TimeSpan.Zero
            };
        })
        .AddJwtBearer("Local", options =>
        {
            options.MapInboundClaims = false;

            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidIssuer = localIdentityOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = $"api://{idAManOptions.ObjectId}",
                ValidateLifetime = true,
                ValidateIssuerSigningKey = false,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(localIdentityOptions.Secret)
                ),
                RoleClaimType = "role",
                NameClaimType = JwtRegisteredClaimNames.Name,
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = ctx =>
                {
                    //Add scope from permission
                    var identity = ctx.Principal?.Identity as ClaimsIdentity;

                    var permissions = identity?.FindAll("permission").ToList();

                    if (permissions?.Count > 0)
                    {
                        foreach (var p in permissions)
                        {
                            identity!.AddClaim(new Claim("scope", p.Value));
                        }
                    }

                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }
}
