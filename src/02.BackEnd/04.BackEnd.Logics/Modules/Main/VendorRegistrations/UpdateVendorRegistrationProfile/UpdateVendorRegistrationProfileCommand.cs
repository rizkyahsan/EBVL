using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.Common;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.PreRegistration;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.UpdateVendorRegistrationProfile;

public sealed record UpdateVendorRegistrationProfileCommand(Guid RegistrationId, UpdateVendorRegistrationProfileRequest Request) : IRequest<QuestionnaireRuntimeResponse>;

public sealed class UpdateVendorRegistrationProfileCommandValidator : AbstractValidatorBase<UpdateVendorRegistrationProfileCommand>
{
    public UpdateVendorRegistrationProfileCommandValidator()
    {
        _ = RuleFor(x => x.Request.Profile).SetValidator(new PreRegistrationRequestValidator());
    }
}

public sealed class UpdateVendorRegistrationProfileHandler(IDatabaseService databaseService) : IRequestHandler<UpdateVendorRegistrationProfileCommand, QuestionnaireRuntimeResponse>
{
    public async Task<QuestionnaireRuntimeResponse> Handle(UpdateVendorRegistrationProfileCommand request, CancellationToken cancellationToken)
    {
        var registration = await QuestionnaireRuntime.Load(databaseService, request.RegistrationId, request.Request.ResumeToken, true, cancellationToken);
        QuestionnaireRuntime.EnsureDraft(registration);
        QuestionnaireRuntime.SetConcurrency(databaseService, registration, request.Request.RowVersion);
        if (request.Request.Profile.CompanyStatus != registration.CompanyType)
        {
            throw new ValidationException("Company type cannot be changed after registration starts.");
        }

        var normalizedSap = request.Request.Profile.SapVendorNumber.Trim().ToUpperInvariant();
        if (await databaseService.VendorRegistrations.AnyAsync(x => !x.IsDeleted && x.Id != registration.Id && x.NormalizedSapVendorNumber == normalizedSap, cancellationToken))
        {
            throw new ValidationException("An active registration already exists for this SAP vendor number.");
        }

        QuestionnaireRuntime.AssignProfile(registration, request.Request.Profile);
        _ = await databaseService.SaveAsync(nameof(UpdateVendorRegistrationProfileCommand), cancellationToken);

        return await QuestionnaireRuntime.LoadResponse(databaseService, registration.Id, request.Request.ResumeToken, null, cancellationToken);
    }
}
