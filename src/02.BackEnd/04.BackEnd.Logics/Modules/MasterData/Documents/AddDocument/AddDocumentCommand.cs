using EBVL.BackEnd.Logics.Modules.MasterData.Documents.Common;
using EBVL.Shared.Dto.Modules.MasterData.Documents;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Documents.AddDocument;

[AuthorizeRequestByPermission(DocumentPermissions.Manage)]
public sealed record AddDocumentCommand(Guid DocumentRequirementSetId, AddDocumentRequest Document) : IRequest<GetDocumentResponse>;

public sealed class AddDocumentCommandValidator : AbstractValidatorBase<AddDocumentCommand>
{
    public AddDocumentCommandValidator()
    {
        _ = RuleFor(x => x.Document).SetValidator(new AddDocumentRequestValidator());
    }
}

public sealed class AddDocumentHandler(IDatabaseService db) : IRequestHandler<AddDocumentCommand, GetDocumentResponse>
{
    public async Task<GetDocumentResponse> Handle(AddDocumentCommand request, CancellationToken cancellationToken)
    {
        var set = await DocumentRequirementSetGraph.Load(db, request.DocumentRequirementSetId, true, cancellationToken);
        DocumentRequirementSetGraph.EnsureDraft(set);
        DocumentRequirementSetGraph.EnsureRowVersion(set, request.Document.RowVersion);
        DocumentRequirementSetGraph.EnsureProcess(set, request.Document.BusinessProcess);
        var name = request.Document.Name.Trim();
        if (set.Requirements.Any(x => !x.IsDeleted && x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ValidationException("A document type with the same name already exists.");
        }

        set.Modified = DateTimeOffset.UtcNow;
        var active = set.Requirements.Where(x => !x.IsDeleted).ToList();
        var requestedOrder = Math.Clamp(request.Document.Order, 1, active.Count + 1);
        foreach (var item in active.Where(x => x.Order >= requestedOrder))
        {
            item.Order++;
        }

        var document = new DocumentDefinition { Id = Guid.CreateVersion7(), DocumentRequirementSetId = set.Id, Code = DocumentRequirementSetGraph.NextCode(set), BusinessProcess = set.BusinessProcess, Name = name, Order = requestedOrder, MaxSizeMb = request.Document.MaxSizeMb, IsMandatory = request.Document.IsMandatory, IsActive = request.Document.IsActive };
        _ = db.DocumentDefinitions.Add(document);
        await DocumentRequirementSetGraph.Save(db, nameof(AddDocumentCommand), cancellationToken);
        return new() { Item = DocumentRequirementSetGraph.Map(set) };
    }
}
