namespace EBVL.BackEnd.Infrastructure.LocalIdentity;

public sealed record LocalIdentityOptions
{
    public required string Secret { get; set; }

    public required string Key { get; set; }

    public required string Issuer { get; set; }
}
