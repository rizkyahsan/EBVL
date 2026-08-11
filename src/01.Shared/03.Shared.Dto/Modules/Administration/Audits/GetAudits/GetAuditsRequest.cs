namespace EBVL.Shared.Dto.Modules.Administration.Audits.GetAudits;

public record GetAuditsRequest : PaginatedListRequest
{
    public DateTimeOffset? From { get; set; }
    public DateTimeOffset? To { get; set; }
}

public sealed class GetAuditsRequestValidator : AbstractValidatorBase<GetAuditsRequest>
{
    public GetAuditsRequestValidator()
    {
        Include(new PaginatedListRequestValidator());

        _ = RuleFor(x => x.To)
            .GreaterThanOrEqualTo(x => x.From);
    }
}
