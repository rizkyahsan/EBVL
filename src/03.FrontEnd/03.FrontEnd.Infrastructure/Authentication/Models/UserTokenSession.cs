namespace EBVL.FrontEnd.Infrastructure.Authentication.Models;

public sealed class UserTokenSession
{
    public required string UserToken { get; set; }

    public required DateTimeOffset ExpiredAt { get; set; }

    public required string IpAddress { get; set; }

    public required string UserAgent { get; set; }
}
