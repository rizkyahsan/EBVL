using EBVL.Shared.Dto.Modules.Examples.Documents.DownloadDocument;

namespace EBVL.FrontEnd.Logics.Modules.Examples.Documents.DownloadDocument;

public sealed record DownloadDocumentQuery : DownloadDocumentRequest, IRequest<DownloadDocumentResponse>
{
}

public sealed class DownloadDocumentQueryValidator : AbstractValidatorBase<DownloadDocumentQuery>
{
    public DownloadDocumentQueryValidator()
    {
        Include(new DownloadDocumentRequestValidator());
    }
}

public sealed class DownloadDocumentQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<DownloadDocumentQuery, DownloadDocumentResponse>
{
    public async Task<DownloadDocumentResponse> Handle(DownloadDocumentQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(DownloadDocumentRoute.ResourceUri(request.DocumentId), Method.Get);

        return await backEndApiService.SendRequestAsync<DownloadDocumentResponse>(restRequest, cancellationToken);
    }
}
