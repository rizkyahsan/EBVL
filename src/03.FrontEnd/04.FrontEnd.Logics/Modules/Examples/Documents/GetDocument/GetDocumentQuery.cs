using EBVL.Shared.Dto.Modules.Examples.Documents.GetDocument;

namespace EBVL.FrontEnd.Logics.Modules.Examples.Documents.GetDocument;

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

public sealed class GetDocumentQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetDocumentQuery, GetDocumentResponse>
{
    public async Task<GetDocumentResponse> Handle(GetDocumentQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetDocumentRoute.ResourceUri(request.DocumentId), Method.Get);

        return await backEndApiService.SendRequestAsync<GetDocumentResponse>(restRequest, cancellationToken);
    }
}
