namespace EBVL.Shared.Dto.Modules.Administration.Audits.GetAudit;

public record GetAuditRequest
{
    public required Guid AuditId { get; init; }
}

public sealed class GetAuditRequestValidator : AbstractValidatorBase<GetAuditRequest>
{
    public GetAuditRequestValidator()
    {
        _ = RuleFor(x => x.AuditId)
            .NotEmpty();
    }
}
