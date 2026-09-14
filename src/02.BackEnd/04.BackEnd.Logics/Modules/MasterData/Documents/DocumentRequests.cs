using EBVL.Shared.Dto.Modules.MasterData.Documents;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Documents;

// TODO: Restore document permissions after IdAMan permission setup is complete.
public sealed record GetDocumentsQuery : IRequest<GetDocumentsResponse>;
public sealed record GetDocumentQuery(Guid DocumentId) : IRequest<GetDocumentResponse>;
public sealed record AddDocumentCommand : AddDocumentRequest, IRequest<GetDocumentResponse>;
public sealed record UpdateDocumentCommand(Guid DocumentId, UpdateDocumentRequest Document) : IRequest<GetDocumentResponse>;

public sealed class AddDocumentCommandValidator : AbstractValidatorBase<AddDocumentCommand>
{
    public AddDocumentCommandValidator()
    {
        Include(new AddDocumentRequestValidator());
    }
}

public sealed class UpdateDocumentCommandValidator : AbstractValidatorBase<UpdateDocumentCommand>
{
    public UpdateDocumentCommandValidator()
    {
        _ = RuleFor(x => x.Document).SetValidator(new UpdateDocumentRequestValidator());
    }
}

public sealed class GetDocumentsHandler(IDatabaseService db) : IRequestHandler<GetDocumentsQuery, GetDocumentsResponse>
{
    public async Task<GetDocumentsResponse> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
    {
        var items = await db.DocumentDefinitions.AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.BusinessProcess)
            .ThenBy(x => x.Name)
            .Select(x => new DocumentListItem(x.Id, x.BusinessProcess, x.Name, x.MaxSizeMb, x.IsMandatory, x.IsActive, Convert.ToBase64String(x.RowVersion)))
            .ToListAsync(cancellationToken);
        return new() { Items = items };
    }
}

public sealed class GetDocumentHandler(IDatabaseService db) : IRequestHandler<GetDocumentQuery, GetDocumentResponse>
{
    public async Task<GetDocumentResponse> Handle(GetDocumentQuery request, CancellationToken cancellationToken)
    {
        var item = await db.DocumentDefinitions.AsNoTracking()
            .Where(x => x.Id == request.DocumentId && !x.IsDeleted)
            .Select(x => new DocumentListItem(x.Id, x.BusinessProcess, x.Name, x.MaxSizeMb, x.IsMandatory, x.IsActive, Convert.ToBase64String(x.RowVersion)))
            .SingleOrDefaultAsync(cancellationToken) ?? throw new InvalidOperationException("Document definition was not found.");
        return new() { Item = item };
    }
}

public sealed class AddDocumentHandler(IDatabaseService db) : IRequestHandler<AddDocumentCommand, GetDocumentResponse>
{
    public async Task<GetDocumentResponse> Handle(AddDocumentCommand request, CancellationToken cancellationToken)
    {
        var businessProcess = request.BusinessProcess.Trim();
        var name = request.Name.Trim();
        if (await db.DocumentDefinitions.AnyAsync(x => !x.IsDeleted && x.BusinessProcess == businessProcess && x.Name == name, cancellationToken))
        {
            throw new ValidationException("A document type with the same business process and name already exists.");
        }

        var document = new DocumentDefinition
        {
            BusinessProcess = businessProcess,
            Name = name,
            MaxSizeMb = request.MaxSizeMb,
            IsMandatory = request.IsMandatory,
            IsActive = request.IsActive
        };
        _ = await db.DocumentDefinitions.AddAsync(document, cancellationToken);
        _ = await db.SaveAsync(nameof(AddDocumentCommand), cancellationToken);
        return new() { Item = Map(document) };
    }

    private static DocumentListItem Map(DocumentDefinition x)
    {
        return new(x.Id, x.BusinessProcess, x.Name, x.MaxSizeMb, x.IsMandatory, x.IsActive, Convert.ToBase64String(x.RowVersion));
    }
}

public sealed class UpdateDocumentHandler(IDatabaseService db) : IRequestHandler<UpdateDocumentCommand, GetDocumentResponse>
{
    public async Task<GetDocumentResponse> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await db.DocumentDefinitions.SingleOrDefaultAsync(x => x.Id == request.DocumentId && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Document definition was not found.");
        var businessProcess = request.Document.BusinessProcess.Trim();
        var name = request.Document.Name.Trim();
        if (await db.DocumentDefinitions.AnyAsync(x => x.Id != document.Id && !x.IsDeleted && x.BusinessProcess == businessProcess && x.Name == name, cancellationToken))
        {
            throw new ValidationException("A document type with the same business process and name already exists.");
        }

        db.SetDocumentDefinitionOriginalRowVersion(document, Convert.FromBase64String(request.Document.RowVersion));
        document.BusinessProcess = businessProcess;
        document.Name = name;
        document.MaxSizeMb = request.Document.MaxSizeMb;
        document.IsMandatory = request.Document.IsMandatory;
        document.IsActive = request.Document.IsActive;
        _ = await db.SaveAsync(nameof(UpdateDocumentCommand), cancellationToken);
        return new() { Item = new(document.Id, document.BusinessProcess, document.Name, document.MaxSizeMb, document.IsMandatory, document.IsActive, Convert.ToBase64String(document.RowVersion)) };
    }
}
