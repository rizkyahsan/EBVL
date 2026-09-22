using EBVL.BackEnd.Logics.Modules.MasterData.Documents.Common;
using EBVL.Shared.Dto.Modules.MasterData.Documents;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Documents.CreateDocumentDraft;

[AuthorizeRequestByPermission(DocumentPermissions.Manage)]
public sealed record CreateDocumentDraftCommand(Guid DocumentRequirementSetId) : IRequest<GetDocumentResponse>;

public sealed class CreateDocumentDraftHandler(IDatabaseService db) : IRequestHandler<CreateDocumentDraftCommand, GetDocumentResponse>
{
    public async Task<GetDocumentResponse> Handle(CreateDocumentDraftCommand request, CancellationToken cancellationToken)
    {
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var source = await DocumentRequirementSetGraph.Load(db, request.DocumentRequirementSetId, true, cancellationToken);
        var existing = await db.DocumentRequirementSets.Include(x => x.Requirements).SingleOrDefaultAsync(x => !x.IsDeleted && x.DocumentRequirementSetSeriesId == source.DocumentRequirementSetSeriesId && x.Status == QuestionnaireStatus.Draft, cancellationToken);
        if (existing is not null)
        {
            await tx.CommitAsync(cancellationToken);
            return new() { Item = DocumentRequirementSetGraph.Map(existing) };
        }

        if (source.Status != QuestionnaireStatus.Publish)
        {
            throw new ValidationException("A draft can only be created from the published document requirement set.");
        }

        var version = await db.DocumentRequirementSets.Where(x => x.DocumentRequirementSetSeriesId == source.DocumentRequirementSetSeriesId).MaxAsync(x => x.Version, cancellationToken) + 1;
        var draft = DocumentRequirementSetGraph.Clone(source, version);
        _ = db.DocumentRequirementSets.Add(draft);
        _ = await db.SaveAsync(nameof(CreateDocumentDraftCommand), cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return new() { Item = DocumentRequirementSetGraph.Map(draft) };
    }
}
