namespace EBVL.Shared.Dto.Modules.Administration.ApiCalls.GetApiCall;

public record GetApiCallRequest
{
    public required Guid ApiCallId { get; init; }
}

public sealed class GetApiCallRequestValidator : AbstractValidatorBase<GetApiCallRequest>
{
    public GetApiCallRequestValidator()
    {
        _ = RuleFor(x => x.ApiCallId)
            .NotEmpty();
    }
}
