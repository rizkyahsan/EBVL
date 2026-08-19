namespace EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.DeleteEmailTemplate;

public record DeleteEmailTemplateRequest
{
    public required Guid EmailTemplateId { get; init; }
}

public sealed class DeleteEmailTemplateRequestValidator : AbstractValidatorBase<DeleteEmailTemplateRequest>
{
    public DeleteEmailTemplateRequestValidator()
    {
        _ = RuleFor(x => x.EmailTemplateId)
            .NotEmpty();
    }
}
