using EBVL.BackEnd.Logics.Modules.MasterData.Documents.Common;
using EBVL.Shared.Dto.Modules.MasterData.Documents;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Documents.GetDocument;

[AuthorizeRequest]
public sealed record GetDocumentQuery(Guid DocumentRequirementSetId) : IRequest<GetDocumentResponse>;

public sealed class GetDocumentHandler(IDatabaseService db) : IRequestHandler<GetDocumentQuery, GetDocumentResponse>
{
    public async Task<GetDocumentResponse> Handle(GetDocumentQuery request, CancellationToken cancellationToken)
    {
        return new() { Item = DocumentRequirementSetGraph.Map(await DocumentRequirementSetGraph.Load(db, request.DocumentRequirementSetId, false, cancellationToken)) };
    }
}
