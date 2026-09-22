using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.Common;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.PreRegistration;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;
using Pertamina.Services.CurrentUser;

namespace EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.StartQuestionnaire;

public sealed record StartQuestionnaireCommand(PreRegistrationRequest Request) : IRequest<QuestionnaireRuntimeResponse>;

public sealed class StartQuestionnaireCommandValidator : AbstractValidatorBase<StartQuestionnaireCommand>
{
    public StartQuestionnaireCommandValidator()
    {
        _ = RuleFor(x => x.Request).SetValidator(new PreRegistrationRequestValidator());
    }
}

public sealed class StartQuestionnaireHandler(IDatabaseService databaseService, ICurrentUserService currentUser) : IRequestHandler<StartQuestionnaireCommand, QuestionnaireRuntimeResponse>
{
    public async Task<QuestionnaireRuntimeResponse> Handle(StartQuestionnaireCommand request, CancellationToken cancellationToken)
    {
        var profile = request.Request;
        var normalizedSap = profile.SapVendorNumber.Trim().ToUpperInvariant();
        var userId = currentUser.Username is null
            ? null
            : await databaseService.Users.Where(x => !x.IsDeleted && x.Username == currentUser.Username).Select(x => (Guid?)x.Id).SingleOrDefaultAsync(cancellationToken);
        if (userId is not null && await databaseService.VendorRegistrations.AnyAsync(x => !x.IsDeleted && x.UserId == userId, cancellationToken))
        {
            throw new ValidationException("An active registration already exists for this account.");
        }

        if (normalizedSap is not null && await databaseService.VendorRegistrations.AnyAsync(x => !x.IsDeleted && x.NormalizedSapVendorNumber == normalizedSap, cancellationToken))
        {
            throw new ValidationException("An active registration already exists for this SAP vendor number.");
        }

        var questionnaire = await databaseService.Questionnaires.Include(x => x.Sections).ThenInclude(x => x.Questions).ThenInclude(x => x.Options)
            .Include(x => x.Rules).SingleOrDefaultAsync(x => !x.IsDeleted && x.Status == QuestionnaireStatus.Publish && x.IsActive && x.Code == "VENDOR_REGISTRATION", cancellationToken)
            ?? throw new ValidationException("No published vendor registration questionnaire is available. Registration cannot be started.");
        var documentRequirementSet = await databaseService.DocumentRequirementSets.SingleOrDefaultAsync(x => !x.IsDeleted && x.Status == QuestionnaireStatus.Publish && x.BusinessProcess == "Vendor Registration", cancellationToken)
            ?? throw new ValidationException("No published vendor registration document requirement set is available. Registration cannot be started.");
        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        var token = Convert.ToBase64String(tokenBytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        var registration = new VendorRegistration
        {
            UserId = userId,
            CompanyType = profile.CompanyStatus!.Value,
            SapVendorNumber = profile.SapVendorNumber.Trim(),
            NormalizedSapVendorNumber = normalizedSap,
            CompanyName = profile.CompanyName.Trim(),
            CompanyEmail = profile.CompanyEmail.Trim(),
            PicEmail = profile.PicEmail.Trim(),
            CompanyPhoneNumber = profile.CompanyPhoneNumber.Trim(),
            PicPhoneNumber = profile.PicPhoneNumber.Trim(),
            Website = profile.Website.Trim(),
            CompanyService = profile.CompanyService!.Value,
            FactoryCountry = profile.FactoryCountry.Trim(),
            FactoryAddress = profile.FactoryAddress.Trim(),
            BrandRepresentative = profile.BrandRepresentative.Trim(),
            AdditionalBrandsJson = JsonSerializer.Serialize(profile.AdditionalBrands),
            IsRepresentativeInIndonesia = profile.IsRepresentativeInIndonesia!.Value,
            RepresentativeName = profile.RepresentativeName.Trim(),
            ResumeTokenHash = SHA256.HashData(Encoding.UTF8.GetBytes(token)),
            ResumeTokenExpiresAt = DateTimeOffset.UtcNow.AddDays(30),
            Status = VendorRegistrationStatus.Draft,
            QuestionnaireId = questionnaire.Id,
            DocumentRequirementSetId = documentRequirementSet.Id
        };
        _ = await databaseService.VendorRegistrations.AddAsync(registration, cancellationToken);
        _ = await databaseService.SaveAsync(nameof(StartQuestionnaireCommand), cancellationToken);

        return await QuestionnaireRuntime.LoadResponse(databaseService, registration.Id, token, token, cancellationToken);
    }
}
