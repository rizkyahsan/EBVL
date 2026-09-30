using EBVL.Shared.Enums;

namespace EBVL.Shared.Dto.Modules.Main.BrandRegistrations.RespondBrandInvitation;

public sealed record RespondBrandInvitationRequest(BrandInvitationResponse Response, string RowVersion);

public sealed class RespondBrandInvitationRequestValidator : AbstractValidator<RespondBrandInvitationRequest>
{
    public RespondBrandInvitationRequestValidator()
    {
        _ = RuleFor(x => x.Response).IsInEnum();
        _ = RuleFor(x => x.RowVersion).NotEmpty();
    }
}
