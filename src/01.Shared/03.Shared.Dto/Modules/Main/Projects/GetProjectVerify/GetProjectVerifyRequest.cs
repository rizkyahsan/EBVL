namespace EBVL.Shared.Dto.Modules.Main.Projects.GetProjectVerify;

public record GetProjectVerifyRequest
{
    public required Guid Id { get; init; }
}

public sealed class GetProjectVerifyRequestValidator : AbstractValidatorBase<GetProjectVerifyRequest>
{
    public GetProjectVerifyRequestValidator()
    {
        _ = RuleFor(x => x.Id)
            .NotEmpty();
    }
}
