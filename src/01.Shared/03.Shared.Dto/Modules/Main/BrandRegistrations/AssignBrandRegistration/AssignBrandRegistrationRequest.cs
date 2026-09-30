namespace EBVL.Shared.Dto.Modules.Main.BrandRegistrations.AssignBrandRegistration;

public sealed record AssignBrandRegistrationRequest(string AssigneeUsername, string RowVersion, string? Note);

public sealed class AssignBrandRegistrationRequestValidator : AbstractValidator<AssignBrandRegistrationRequest>
{
    public AssignBrandRegistrationRequestValidator()
    {
        _ = RuleFor(x => x.AssigneeUsername).NotEmpty().MaximumLength(320).EmailAddress();
        _ = RuleFor(x => x.RowVersion).NotEmpty();
        _ = RuleFor(x => x.Note).MaximumLength(2000);
    }
}
