using EBVL.BackEnd.Logics.Modules.MasterData.Documents.Common;
using EBVL.Shared.Dto.Modules.MasterData.Documents;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Documents.UpdateDocument;

[AuthorizeRequestByPermission(DocumentPermissions.Manage)]
public sealed record UpdateDocumentCommand(Guid DocumentRequirementSetId, Guid DocumentId, UpdateDocumentRequest Document) : IRequest<GetDocumentResponse>;

public sealed class UpdateDocumentCommandValidator : AbstractValidatorBase<UpdateDocumentCommand>
{
    public UpdateDocumentCommandValidator()
    {
        _ = RuleFor(x => x.Document).SetValidator(new UpdateDocumentRequestValidator());
    }
}

public sealed class UpdateDocumentHandler(IDatabaseService db) : IRequestHandler<UpdateDocumentCommand, GetDocumentResponse>
{
    public async Task<GetDocumentResponse> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
    {
        var set = await DocumentRequirementSetGraph.Load(db, request.DocumentRequirementSetId, true, cancellationToken);
        DocumentRequirementSetGraph.EnsureDraft(set);
        DocumentRequirementSetGraph.EnsureRowVersion(set, request.Document.RowVersion);
        DocumentRequirementSetGraph.EnsureProcess(set, request.Document.BusinessProcess);
        var document = set.Requirements.SingleOrDefault(x => x.Id == request.DocumentId && !x.IsDeleted) ?? throw new KeyNotFoundException("Document requirement was not found.");
        var name = request.Document.Name.Trim();
        if (set.Requirements.Any(x => x.Id != document.Id && !x.IsDeleted && x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ValidationException("A document type with the same name already exists.");
        }

        document.Name = name;
        document.MaxSizeMb = request.Document.MaxSizeMb;
        document.IsMandatory = request.Document.IsMandatory;
        document.IsActive = request.Document.IsActive;
        var ordered = set.Requirements.Where(x => !x.IsDeleted && x.Id != document.Id).OrderBy(x => x.Order).ToList();
        ordered.Insert(Math.Clamp(request.Document.Order - 1, 0, ordered.Count), document);
        for (var index = 0; index < ordered.Count; index++)
        {
            ordered[index].Order = index + 1;
        }

        set.Modified = DateTimeOffset.UtcNow;
        await DocumentRequirementSetGraph.Save(db, nameof(UpdateDocumentCommand), cancellationToken);
        return new() { Item = DocumentRequirementSetGraph.Map(set) };
    }
}
