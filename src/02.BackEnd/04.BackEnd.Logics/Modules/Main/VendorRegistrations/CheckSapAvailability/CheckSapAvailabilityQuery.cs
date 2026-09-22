using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.CheckSapAvailability;

public sealed record CheckSapAvailabilityQuery(string SapVendorNumber, Guid? RegistrationId) : IRequest<SapAvailabilityResponse>;

public sealed class CheckSapAvailabilityHandler(IDatabaseService databaseService) : IRequestHandler<CheckSapAvailabilityQuery, SapAvailabilityResponse>
{
    public async Task<SapAvailabilityResponse> Handle(CheckSapAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var normalizedSap = request.SapVendorNumber.Trim().ToUpperInvariant();
        var exists = await databaseService.VendorRegistrations.AsNoTracking().AnyAsync(x => !x.IsDeleted
            && x.Id != request.RegistrationId
            && x.NormalizedSapVendorNumber == normalizedSap, cancellationToken);

        return exists
            ? new(false, "This SAP vendor number is already used by an active registration.")
            : new(true, null);
    }
}
