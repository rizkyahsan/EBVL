using EBVL.BackEnd.Services.SapVendor;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;
using VendorRegistrationMaximumLengthFor = EBVL.Shared.Statics.VendorRegistrations.MaximumLengthFor;

namespace EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.CheckSapAvailability;

public sealed record CheckSapAvailabilityQuery(string SapVendorNumber) : IRequest<SapAvailabilityResponse>;

public sealed class CheckSapAvailabilityQueryValidator : AbstractValidator<CheckSapAvailabilityQuery>
{
    public CheckSapAvailabilityQueryValidator()
    {
        _ = RuleFor(x => x.SapVendorNumber).NotEmpty().MaximumLength(VendorRegistrationMaximumLengthFor.SapVendorNumber);
    }
}

public sealed class CheckSapAvailabilityHandler(IDatabaseService databaseService, ISapVendorService sapVendorService) : IRequestHandler<CheckSapAvailabilityQuery, SapAvailabilityResponse>
{
    public async Task<SapAvailabilityResponse> Handle(CheckSapAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var sapVendorNumber = request.SapVendorNumber.Trim();
        var normalizedSap = sapVendorNumber.ToUpperInvariant();
        var isRegistered = await databaseService.VendorRegistrations.AnyAsync(x => !x.IsDeleted
            && (x.NormalizedSapVendorNumber == normalizedSap
                || (x.SapVendorNumber != null && x.SapVendorNumber.Trim() == sapVendorNumber)), cancellationToken);
        if (isRegistered)
        {
            return new(false, false, sapVendorNumber, "This SAP vendor number already has an active registration.");
        }

        var lookup = await sapVendorService.LookupAsync(sapVendorNumber, cancellationToken);
        return lookup.Status == SapVendorLookupStatus.Found
            ? new(true, true, lookup.SapVendorNumber!, null)
            : new(true, false, sapVendorNumber, lookup.Message);
    }
}
