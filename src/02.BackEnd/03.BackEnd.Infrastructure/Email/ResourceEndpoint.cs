using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace EBVL.BackEnd.Infrastructure.Email;

public sealed record ResourceEndpoint
{
    public required string SendEmailNoTemplate { get; init; }

    public required string Status { get; init; }

    [CompilerGenerated]
    [SetsRequiredMembers]
    private ResourceEndpoint(ResourceEndpoint original)
    {
        SendEmailNoTemplate = original.SendEmailNoTemplate;
        Status = original.Status;
    }

    public ResourceEndpoint()
    {
    }
}
