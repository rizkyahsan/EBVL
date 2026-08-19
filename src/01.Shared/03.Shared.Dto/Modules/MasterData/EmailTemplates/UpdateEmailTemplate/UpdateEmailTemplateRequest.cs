namespace EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.UpdateEmailTemplate;

public record UpdateEmailTemplateRequest
{
    public required Guid EmailTemplateId { get; init; }
    public string DefaultTo { get; set; } = string.Empty;
    public string DefaultCc { get; set; } = string.Empty;
    public required string Subject { get; set; }
    public required string Content { get; set; }
}

public sealed class UpdateEmailTemplateRequestValidator : AbstractValidatorBase<UpdateEmailTemplateRequest>
{
    public UpdateEmailTemplateRequestValidator()
    {
        _ = RuleFor(x => x.EmailTemplateId)
            .NotEmpty();

        _ = RuleFor(x => x.Subject)
            .NotEmpty();

        _ = RuleFor(x => x.Content)
            .NotEmpty();
    }
}
