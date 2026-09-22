using EBVL.BackEnd.Logics.Modules.MasterData.Documents.Common;
using EBVL.Shared.Dto.Modules.MasterData.Documents;
using Pertamina.Services.CurrentUser;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Documents.PublishDocumentRequirementSet;

[AuthorizeRequestByPermission(DocumentPermissions.Manage)]
public sealed record PublishDocumentRequirementSetCommand(Guid DocumentRequirementSetId, string RowVersion) : IRequest<GetDocumentResponse>;

public sealed class PublishDocumentRequirementSetHandler(IDatabaseService db, ICurrentUserService currentUser) : IRequestHandler<PublishDocumentRequirementSetCommand, GetDocumentResponse>
{
    public async Task<GetDocumentResponse> Handle(PublishDocumentRequirementSetCommand request, CancellationToken cancellationToken)
    {
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var draft = await DocumentRequirementSetGraph.Load(db, request.DocumentRequirementSetId, true, cancellationToken);
        DocumentRequirementSetGraph.EnsureDraft(draft);
        DocumentRequirementSetGraph.EnsureRowVersion(draft, request.RowVersion);
        if (!draft.Requirements.Any(x => !x.IsDeleted && x.IsActive))
        {
            throw new ValidationException("At least one active document requirement is required.");
        }

        var current = await db.DocumentRequirementSets.SingleOrDefaultAsync(x => !x.IsDeleted && x.DocumentRequirementSetSeriesId == draft.DocumentRequirementSetSeriesId && x.Status == QuestionnaireStatus.Publish, cancellationToken);
        try
        {
            if (current is { } published)
            {
                published.Status = QuestionnaireStatus.Superseded;
                _ = await db.SaveAsync(nameof(PublishDocumentRequirementSetCommand), cancellationToken);
            }

            draft.Status = QuestionnaireStatus.Publish;
            draft.PublishedAt = DateTimeOffset.UtcNow;
            draft.PublishedBy = currentUser.Username ?? "EBVLSystem";
            _ = await db.SaveAsync(nameof(PublishDocumentRequirementSetCommand), cancellationToken);
        }
        catch (DbUpdateException)
        {
            throw new ValidationException("The document requirement set changed or was published concurrently. Reload and try again.");
        }

        await tx.CommitAsync(cancellationToken);
        return new() { Item = DocumentRequirementSetGraph.Map(draft) };
    }
}
