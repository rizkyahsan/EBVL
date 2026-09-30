using EBVL.Shared.Enums;

namespace EBVL.Shared.Dto.Modules.Main.BrandRegistrations.TransitionBrandRegistration;

public sealed record TransitionBrandRegistrationRequest(BrandRegistrationAction Action, string RowVersion, string? Note);

public sealed class TransitionBrandRegistrationRequestValidator : AbstractValidator<TransitionBrandRegistrationRequest>
{
    public TransitionBrandRegistrationRequestValidator()
    {
        _ = RuleFor(x => x.Action).IsInEnum();
        _ = RuleFor(x => x.RowVersion).NotEmpty().Must(BeBase64).WithMessage("RowVersion must be valid Base64.");
        _ = RuleFor(x => x.Note).MaximumLength(2000);
    }

    private static bool BeBase64(string value)
    {
        return Convert.TryFromBase64String(value, new Span<byte>(new byte[value.Length]), out _);
    }
}
