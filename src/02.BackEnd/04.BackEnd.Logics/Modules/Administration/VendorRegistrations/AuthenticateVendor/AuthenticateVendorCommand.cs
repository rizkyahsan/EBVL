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
            .AsNoTracking()
            .FirstOrDefaultAsync(x => !x.IsDeleted && x.IsActive && x.Status == VendorAccountStatus.Active && x.VendorId != null && x.EmailAddress == email, cancellationToken);

        if (account is null || !PasswordHasher.Verify(request.Password, account.PasswordHash, account.PasswordSalt))
        {
            throw new UnauthorizedAccessException("Email atau password tidak valid.");
        }

        var vendor = await databaseService.Vendors
            .AsNoTracking()
            .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == account.VendorId, cancellationToken);
        _ = vendor ?? throw new UnauthorizedAccessException("Email atau password tidak valid.");

        var registrationStatus = account.VendorRegistrationId is null
            ? VendorRegistrationStatus.Verified
            : await databaseService.VendorRegistrations
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == account.VendorRegistrationId)
                .Select(x => (VendorRegistrationStatus?)x.Status)
                .FirstOrDefaultAsync(cancellationToken) ?? VendorRegistrationStatus.Verified;

        return new AuthenticateVendorResponse
        {
            VendorAccountId = account.Id,
            VendorRegistrationId = account.VendorRegistrationId ?? Guid.Empty,
            VendorId = account.VendorId!.Value,
            EmailAddress = account.EmailAddress,
            CompanyName = vendor.Name,
            SapVendorNumber = vendor.SapVendorNumber,
            Status = registrationStatus
        };
    }
}
