namespace EBVL.Shared.Dto.Modules.Main.Vendor.RequestRegistrations.GetRequestRegistrations;

public record GetRequestRegistrationsRequest : PaginatedListRequest
{
    public RequestRegistrationCategory? Category { get; set; }
}

public sealed class GetRequestRegistrationsRequestValidator : AbstractValidatorBase<GetRequestRegistrationsRequest>
{
    public GetRequestRegistrationsRequestValidator()
    {
        Include(new PaginatedListRequestValidator());
        _ = RuleFor(x => x.Category).IsInEnum();
    }
}
