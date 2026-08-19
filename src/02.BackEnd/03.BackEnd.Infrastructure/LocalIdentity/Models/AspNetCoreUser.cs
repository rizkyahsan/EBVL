using Microsoft.AspNetCore.Identity;

namespace EBVL.BackEnd.Infrastructure.LocalIdentity.Models;

public class AspNetCoreUser : IdentityUser<Guid>
{
    public required bool IsDeactivated { get; set; }

    public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;

    public string CreatedBy { get; set; } = "EBVLSystem";

    public DateTimeOffset? LastPasswordModified { get; set; }

    public string? LastPasswordModifiedBy { get; set; }

    public DateTimeOffset? Modified { get; set; }

    public string? ModifiedBy { get; set; }
}
