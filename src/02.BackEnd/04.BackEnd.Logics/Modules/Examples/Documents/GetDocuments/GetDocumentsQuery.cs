using EBVL.Shared.Dto.Modules.Examples.Documents.GetDocuments;

namespace EBVL.BackEnd.Logics.Modules.Examples.Documents.GetDocuments;

[AuthorizeRequest]
public sealed record GetDocumentsQuery : IRequest<GetDocumentsResponse>
{
}

public sealed class GetDocumentsQueryHandler(IDatabaseService databaseService)
    : IRequestHandler<GetDocumentsQuery, GetDocumentsResponse>
{
    public async Task<GetDocumentsResponse> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
    {
        var documents = await databaseService.Documents
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.OriginalFileName)
            .Select(x => new DocumentItem
            {
                Id = x.Id,
                Created = x.Created,
                CreatedBy = x.CreatedBy,
                FileName = x.OriginalFileName,
                FileSize = x.FileSize
            })
            .ToListAsync(cancellationToken);

        return new GetDocumentsResponse
        {
            Items = documents
        };
    }
}
