using EBVL.Shared.Dto.Modules.Examples.Documents.GetDocuments;

namespace EBVL.FrontEnd.Logics.Modules.Examples.Documents.GetDocuments;

public sealed record GetDocumentsQuery : IRequest<GetDocumentsResponse>
{
}

public sealed class GetDocumentsQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetDocumentsQuery, GetDocumentsResponse>
{
    public async Task<GetDocumentsResponse> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetDocumentsRoute.ResourceUri, Method.Get);

        return await backEndApiService.SendRequestAsync<GetDocumentsResponse>(restRequest, cancellationToken);
    }
}
