using System.Security.Cryptography;
using System.Text;
using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.Common;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.CheckSapAvailability;

public sealed record CheckSapAvailabilityQuery(string SapVendorNumber, Guid? RegistrationId) : IRequest<SapAvailabilityResponse>;

public sealed class CheckSapAvailabilityHandler(IDatabaseService databaseService) : IRequestHandler<CheckSapAvailabilityQuery, SapAvailabilityResponse>
{
    public async Task<SapAvailabilityResponse> Handle(CheckSapAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var sapVendorNumber = request.SapVendorNumber.Trim();
        var normalizedSap = sapVendorNumber.ToUpperInvariant();
        var registration = await databaseService.VendorRegistrations.SingleOrDefaultAsync(x => !x.IsDeleted
            && x.Id != request.RegistrationId
            && (x.NormalizedSapVendorNumber == normalizedSap
                || (x.SapVendorNumber != null && x.SapVendorNumber.Trim() == sapVendorNumber)), cancellationToken);

        if (registration is null)
        {
            return new(true, null);
        }

        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        var token = Convert.ToBase64String(tokenBytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        registration.NormalizedSapVendorNumber = normalizedSap;
        registration.ResumeTokenHash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        registration.ResumeTokenExpiresAt = DateTimeOffset.UtcNow.AddDays(30);
        _ = await databaseService.SaveAsync(nameof(CheckSapAvailabilityQuery), cancellationToken);

        var runtime = await QuestionnaireRuntime.LoadResponse(databaseService, registration.Id, token, token, cancellationToken);
        return new(false, "An existing registration was found.", runtime);
    }
}
