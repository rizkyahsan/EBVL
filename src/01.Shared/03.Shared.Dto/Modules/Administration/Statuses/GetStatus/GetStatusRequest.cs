namespace EBVL.Shared.Dto.Modules.Administration.Statuses.GetStatus;

public record GetStatusRequest
{
    public required Guid StatusId { get; init; }
}

public sealed class GetStatusRequestValidator : AbstractValidatorBase<GetStatusRequest>
{
    public GetStatusRequestValidator()
    {
        _ = RuleFor(x => x.StatusId)
            .NotEmpty();
    }
}
