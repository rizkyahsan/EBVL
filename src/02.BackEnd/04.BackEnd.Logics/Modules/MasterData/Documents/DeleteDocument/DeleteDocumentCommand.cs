using EBVL.BackEnd.Logics.Modules.MasterData.Documents.Common;
using EBVL.Shared.Dto.Modules.MasterData.Documents;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Documents.DeleteDocument;

[AuthorizeRequestByPermission(DocumentPermissions.Manage)]
public sealed record DeleteDocumentCommand(Guid DocumentRequirementSetId, Guid DocumentId, string RowVersion) : IRequest<GetDocumentResponse>;

public sealed class DeleteDocumentHandler(IDatabaseService db) : IRequestHandler<DeleteDocumentCommand, GetDocumentResponse>
{
    public async Task<GetDocumentResponse> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        var set = await DocumentRequirementSetGraph.Load(db, request.DocumentRequirementSetId, true, cancellationToken);
        DocumentRequirementSetGraph.EnsureDraft(set);
        DocumentRequirementSetGraph.EnsureRowVersion(set, request.RowVersion);
        var document = set.Requirements.SingleOrDefault(x => x.Id == request.DocumentId && !x.IsDeleted) ?? throw new KeyNotFoundException("Document requirement was not found.");
        _ = db.DocumentDefinitions.Remove(document);
        _ = set.Requirements.Remove(document);
        set.Modified = DateTimeOffset.UtcNow;
        var order = 1;
        foreach (var item in set.Requirements.Where(x => !x.IsDeleted).OrderBy(x => x.Order))
        {
            item.Order = order++;
        }

        await DocumentRequirementSetGraph.Save(db, nameof(DeleteDocumentCommand), cancellationToken);
        return new() { Item = DocumentRequirementSetGraph.Map(set) };
    }
}
