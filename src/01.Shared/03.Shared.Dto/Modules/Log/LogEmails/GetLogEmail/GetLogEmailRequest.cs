namespace EBVL.Shared.Dto.Modules.Log.LogEmails.GetLogEmail;

public record GetLogEmailRequest
{
    public required Guid Id { get; init; }
}

public sealed class GetLogEmailRequestValidator : AbstractValidatorBase<GetLogEmailRequest>
{
    public GetLogEmailRequestValidator()
    {
        _ = RuleFor(x => x.Id)
            .NotEmpty();
    }
}
