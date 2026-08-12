using EBVL.Shared.Dto.Modules.Administration.VendorRegistrations.AuthenticateVendor;

namespace EBVL.BackEnd.Logics.Modules.Administration.VendorRegistrations.AuthenticateVendor;

public sealed record AuthenticateVendorCommand : AuthenticateVendorRequest, IRequest<AuthenticateVendorResponse>;

public sealed class AuthenticateVendorCommandValidator : AbstractValidatorBase<AuthenticateVendorCommand>
{
    public AuthenticateVendorCommandValidator()
    {
        Include(new AuthenticateVendorRequestValidator());
    }
}

public sealed class AuthenticateVendorCommandHandler(IDatabaseService databaseService)
    : IRequestHandler<AuthenticateVendorCommand, AuthenticateVendorResponse>
{
    public async Task<AuthenticateVendorResponse> Handle(AuthenticateVendorCommand request, CancellationToken cancellationToken)
    {
        var email = request.EmailAddress.Trim().ToLowerInvariant();
        var account = await databaseService.VendorAccounts
            .Include(x => x.VendorRegistration)
            .FirstOrDefaultAsync(x => !x.IsDeleted && x.IsActive && x.EmailAddress == email, cancellationToken);

        if (account is null || !PasswordHasher.Verify(request.Password, account.PasswordHash, account.PasswordSalt))
        {
            throw new UnauthorizedAccessException("Email atau password tidak valid.");
        }

        return new AuthenticateVendorResponse
        {
            VendorAccountId = account.Id,
            VendorRegistrationId = account.VendorRegistrationId,
            EmailAddress = account.EmailAddress,
            CompanyName = account.VendorRegistration.CompanyName,
            SapVendorNumber = account.VendorRegistration.SapVendorNumber,
            Status = account.VendorRegistration.Status
        };
    }
}
