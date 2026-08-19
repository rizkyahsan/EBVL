using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace EBVL.BackEnd.Infrastructure.Email;

public sealed record SendGridOptions
{
    public string ApiKey { get; init; } = string.Empty;
    public string FromEmail { get; init; } = string.Empty;
}
public sealed record EmailBlast2EmailOptions
{
    public required string Scope { get; init; }

    public required string TokenEndpoint { get; init; }

    public required string ClientId { get; set; }

    public required string ClientSecret { get; set; }

    public required string RestBaseUrl { get; init; }

    public required string HealthCheckEndpoint { get; init; }

    public required string ApiPathBase { get; init; }

    public ResourceEndpoint ResourceEndpoint { get; init; } = default!;
    public required string SendGridApiKey { get; set; }

    public string HealthCheckUrl => RestBaseUrl + HealthCheckEndpoint;

    public string ApiBaseUrl => RestBaseUrl + ApiPathBase;

    public const string SectionKey = "Email:EmailBlast2";

    [CompilerGenerated]
    [SetsRequiredMembers]
    private EmailBlast2EmailOptions(EmailBlast2EmailOptions original)
    {
        Scope = original.Scope;
        RestBaseUrl = original.RestBaseUrl;
        TokenEndpoint = original.TokenEndpoint;
        ClientId = original.ClientId;
        ClientSecret = original.ClientSecret;
        HealthCheckEndpoint = original.HealthCheckEndpoint;
        ApiPathBase = original.ApiPathBase;
        ResourceEndpoint = original.ResourceEndpoint;
        SendGridApiKey = original.SendGridApiKey;
    }

    public EmailBlast2EmailOptions()
    {
    }
}
