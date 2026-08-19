using System.Security.Claims;
using EBVL.BackEnd.Services.LocalIdentity.Model;

namespace EBVL.BackEnd.Services.LocalIdentity;

public interface ILocalIdentityService
{
    public Task<Guid> CreateUserAsync(string username, string email, string password);
    public Task<bool> CheckAccessAsync(Guid id, string token);
    public Task<Guid> VerifyUserAsync(Guid id);
    public Task<LoginResult> VerifyUserPasswordAsync(string username, string password);
    public Task<Guid> UpdatePasswordAsync(Guid id, string password);
    public Task DeleteUserAsync(Guid id);
    public Task<LoginResult> LoginAsync(string username);
    public string GenerateToken(IEnumerable<Claim> claims);
}
