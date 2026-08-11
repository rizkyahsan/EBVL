using Microsoft.AspNetCore.Authentication.JwtBearer;
using Pertamina.Services.IdAMan;

namespace EBVL.BackEnd.Infrastructure.Authentication;

public static class ConfigureAuthentication
{
    public static IServiceCollection AddAuthenticationService(this IServiceCollection services, IdAManOptions idAManOptions)
    {
        _ = services.AddScoped<CustomJwtBearerEvents>();

        _ = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = idAManOptions.Authentication.AuthorityUrl;
                options.Audience = $"api://{idAManOptions.ObjectId}";
                options.EventsType = typeof(CustomJwtBearerEvents);
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new()
                {
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidIssuers = [idAManOptions.Authentication.AuthorityUrl],
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }
}
