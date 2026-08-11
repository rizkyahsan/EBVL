using EBVL.Shared.Dto.Modules.Examples.Documents.GetDocument;

namespace EBVL.BackEnd.Logics.Modules.Examples.Documents.GetDocument;

[AuthorizeRequest]
public sealed record GetDocumentQuery : GetDocumentRequest, IRequest<GetDocumentResponse>
{
}

public sealed class GetDocumentQueryValidator : AbstractValidatorBase<GetDocumentQuery>
{
    public GetDocumentQueryValidator()
    {
        Include(new GetDocumentRequestValidator());
    }
}

public sealed class GetDocumentQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<GetDocumentQuery, GetDocumentResponse>
{
    public async Task<GetDocumentResponse> Handle(GetDocumentQuery request, CancellationToken cancellationToken)
    {
        var audits = await databaseService.Audits
            .Where(audit => audit.EntityName == nameof(Document) && audit.EntityId == request.DocumentId)
            .OrderBy(audit => audit.Created)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var document = await databaseService.Documents
            .Where(x => !x.IsDeleted && x.Id == request.DocumentId)
            .Select(x => new DocumentItem
            {
                Id = x.Id,
                Created = x.Created,
                CreatedBy = x.CreatedBy,
                Modified = x.Modified,
                ModifiedBy = x.ModifiedBy,
                Description = x.Description,
                FileName = x.OriginalFileName,
                FileSize = x.FileSize,
                Audits = audits.ToAuditItems<AuditItem>()
            })
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(DocumentsDisplayTextFor.Document, CommonDisplayTextFor.Id, request.DocumentId);

        return new GetDocumentResponse
        {
            Item = document
        };
    }
}
