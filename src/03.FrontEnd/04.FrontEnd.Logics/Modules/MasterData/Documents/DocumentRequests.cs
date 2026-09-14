using EBVL.Shared.Dto.Modules.MasterData.Documents;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.Documents;

public sealed record GetDocumentsQuery : IRequest<GetDocumentsResponse>;
public sealed record GetDocumentQuery(Guid DocumentId) : IRequest<GetDocumentResponse>;
public sealed record AddDocumentCommand : AddDocumentRequest, IRequest<GetDocumentResponse>;
public sealed record UpdateDocumentCommand(Guid DocumentId, UpdateDocumentRequest Document) : IRequest<GetDocumentResponse>;

public sealed class GetDocumentsQueryHandler(IBackEndApiService api) : IRequestHandler<GetDocumentsQuery, GetDocumentsResponse>
{
    public Task<GetDocumentsResponse> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
    {
        return api.SendRequestAsync<GetDocumentsResponse>(new RestRequest(DocumentRoutes.List, Method.Get), cancellationToken);
    }
}

public sealed class GetDocumentQueryHandler(IBackEndApiService api) : IRequestHandler<GetDocumentQuery, GetDocumentResponse>
{
    public Task<GetDocumentResponse> Handle(GetDocumentQuery request, CancellationToken cancellationToken)
    {
        return api.SendRequestAsync<GetDocumentResponse>(new RestRequest(Route(DocumentRoutes.Detail, request.DocumentId), Method.Get), cancellationToken);
    }

    private static string Route(string route, Guid id)
    {
        return route.Replace("{documentId:guid}", id.ToString());
    }
}

public sealed class AddDocumentCommandHandler(IBackEndApiService api) : IRequestHandler<AddDocumentCommand, GetDocumentResponse>
{
    public Task<GetDocumentResponse> Handle(AddDocumentCommand request, CancellationToken cancellationToken)
    {
        return api.SendRequestAsync<GetDocumentResponse>(new RestRequest(DocumentRoutes.Add, Method.Post).AddJsonBody(request), cancellationToken);
    }
}

public sealed class UpdateDocumentCommandHandler(IBackEndApiService api) : IRequestHandler<UpdateDocumentCommand, GetDocumentResponse>
{
    public Task<GetDocumentResponse> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
    {
        return api.SendRequestAsync<GetDocumentResponse>(new RestRequest(Route(DocumentRoutes.Update, request.DocumentId), Method.Put).AddJsonBody(request.Document), cancellationToken);
    }

    private static string Route(string route, Guid id)
    {
        return route.Replace("{documentId:guid}", id.ToString());
    }
}
