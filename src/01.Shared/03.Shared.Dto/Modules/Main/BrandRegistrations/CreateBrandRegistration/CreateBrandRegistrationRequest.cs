namespace EBVL.Shared.Dto.Modules.Main.BrandRegistrations.CreateBrandRegistration;

public sealed record CreateBrandRegistrationRequest(
    string BrandName,
    string ProductName,
    string Group,
    string FactoryCountry,
    string Category,
    string ProductDescription,
    bool Submit = false);

public sealed class CreateBrandRegistrationRequestValidator : AbstractValidator<CreateBrandRegistrationRequest>
{
    public CreateBrandRegistrationRequestValidator()
    {
        _ = RuleFor(x => x.BrandName).MaximumLength(200);
        _ = RuleFor(x => x.ProductName).MaximumLength(200);
        _ = RuleFor(x => x.Group).MaximumLength(200);
        _ = RuleFor(x => x.FactoryCountry).MaximumLength(200);
        _ = RuleFor(x => x.Category).MaximumLength(100);
        _ = RuleFor(x => x.ProductDescription).MaximumLength(2000);
        _ = RuleFor(x => x.BrandName).NotEmpty().When(x => x.Submit);
        _ = RuleFor(x => x.ProductName).NotEmpty().When(x => x.Submit);
        _ = RuleFor(x => x.Group).NotEmpty().When(x => x.Submit);
        _ = RuleFor(x => x.FactoryCountry).NotEmpty().When(x => x.Submit);
        _ = RuleFor(x => x.Category).NotEmpty().When(x => x.Submit);
        _ = RuleFor(x => x.ProductDescription).NotEmpty().When(x => x.Submit);
    }
}
