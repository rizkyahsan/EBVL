namespace EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.GetEmailTemplate;

public record GetEmailTemplateRequest
{
    public required Guid EmailTemplateId { get; init; }
}

public sealed class GetEmailTemplateRequestValidator : AbstractValidatorBase<GetEmailTemplateRequest>
{
    public GetEmailTemplateRequestValidator()
    {
        _ = RuleFor(x => x.EmailTemplateId)
            .NotEmpty();
    }
}
