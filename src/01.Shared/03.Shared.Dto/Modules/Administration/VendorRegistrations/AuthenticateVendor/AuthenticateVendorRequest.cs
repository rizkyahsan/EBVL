namespace EBVL.Shared.Dto.Modules.Administration.VendorRegistrations.AuthenticateVendor;

public record AuthenticateVendorRequest
{
    public required string EmailAddress { get; set; }
    public required string Password { get; set; }
}

public sealed class AuthenticateVendorRequestValidator : AbstractValidatorBase<AuthenticateVendorRequest>
{
    public AuthenticateVendorRequestValidator()
    {
        _ = RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress().MaximumLength(320);
        _ = RuleFor(x => x.Password).NotEmpty().MaximumLength(128);
    }
}
