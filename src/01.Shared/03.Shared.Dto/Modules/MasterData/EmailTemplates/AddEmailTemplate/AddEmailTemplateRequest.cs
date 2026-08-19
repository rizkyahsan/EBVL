namespace EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.AddEmailTemplate;

public record AddEmailTemplateRequest
{
    public required string Module { get; set; }
    public required string Action { get; set; }
    public string DefaultTo { get; set; } = string.Empty;
    public string DefaultCc { get; set; } = string.Empty;
    public required string Subject { get; set; }
    public required string Content { get; set; }
}

public sealed class AddEmailTemplateRequestValidator : AbstractValidatorBase<AddEmailTemplateRequest>
{
    public AddEmailTemplateRequestValidator()
    {
        _ = RuleFor(x => x.Module)
            .NotEmpty()
            .MinimumLength(EmailTemplatesMinimumLengthFor.Module)
            .MaximumLength(EmailTemplatesMaximumLengthFor.Module);

        _ = RuleFor(x => x.Action)
            .NotEmpty()
            .MinimumLength(EmailTemplatesMinimumLengthFor.Action)
            .MaximumLength(EmailTemplatesMaximumLengthFor.Action);

        _ = RuleFor(x => x.Subject)
            .NotEmpty();

        _ = RuleFor(x => x.Content)
            .NotEmpty();
    }
}
