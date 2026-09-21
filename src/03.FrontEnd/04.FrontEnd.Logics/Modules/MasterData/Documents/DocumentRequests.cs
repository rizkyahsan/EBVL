using EBVL.Shared.Dto.Modules.MasterData.Documents;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.Documents;

public sealed record GetDocumentsQuery : IRequest<GetDocumentsResponse>;
public sealed record GetDocumentQuery(Guid DocumentRequirementSetId) : IRequest<GetDocumentResponse>;
public sealed record GetDocumentHistoryQuery(Guid SeriesId) : IRequest<GetDocumentHistoryResponse>;
public sealed record CreateDocumentDraftCommand(Guid DocumentRequirementSetId) : IRequest<GetDocumentResponse>;
public sealed record PublishDocumentRequirementSetCommand(Guid DocumentRequirementSetId, string RowVersion) : IRequest<GetDocumentResponse>;
public sealed record AddDocumentCommand(Guid DocumentRequirementSetId, AddDocumentRequest Document) : IRequest<GetDocumentResponse>;
public sealed record UpdateDocumentCommand(Guid DocumentRequirementSetId, Guid DocumentId, UpdateDocumentRequest Document) : IRequest<GetDocumentResponse>;
public sealed record DeleteDocumentCommand(Guid DocumentRequirementSetId, Guid DocumentId, string RowVersion) : IRequest<GetDocumentResponse>;

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
        return api.SendRequestAsync<GetDocumentResponse>(new RestRequest(Route(DocumentRoutes.Detail, request.DocumentRequirementSetId), Method.Get), cancellationToken);
    }

    internal static string Route(string route, Guid id)
    {
        return route.Replace("{documentRequirementSetId:guid}", id.ToString());
    }
}
public sealed class GetDocumentHistoryQueryHandler(IBackEndApiService api) : IRequestHandler<GetDocumentHistoryQuery, GetDocumentHistoryResponse>
{
    public Task<GetDocumentHistoryResponse> Handle(GetDocumentHistoryQuery request, CancellationToken cancellationToken)
    {
        return api.SendRequestAsync<GetDocumentHistoryResponse>(new RestRequest(DocumentRoutes.History.Replace("{seriesId:guid}", request.SeriesId.ToString()), Method.Get), cancellationToken);
    }
}
public sealed class CreateDocumentDraftCommandHandler(IBackEndApiService api) : IRequestHandler<CreateDocumentDraftCommand, GetDocumentResponse>
{
    public Task<GetDocumentResponse> Handle(CreateDocumentDraftCommand request, CancellationToken cancellationToken)
    {
        return api.SendRequestAsync<GetDocumentResponse>(new RestRequest(GetDocumentQueryHandler.Route(DocumentRoutes.Draft, request.DocumentRequirementSetId), Method.Post), cancellationToken);
    }
}
public sealed class PublishDocumentRequirementSetCommandHandler(IBackEndApiService api) : IRequestHandler<PublishDocumentRequirementSetCommand, GetDocumentResponse>
{
    public Task<GetDocumentResponse> Handle(PublishDocumentRequirementSetCommand request, CancellationToken cancellationToken)
    {
        return api.SendRequestAsync<GetDocumentResponse>(new RestRequest(GetDocumentQueryHandler.Route(DocumentRoutes.Publish, request.DocumentRequirementSetId), Method.Post).AddJsonBody(new PublishDocumentRequirementSetRequest { RowVersion = request.RowVersion }), cancellationToken);
    }
}
public sealed class AddDocumentCommandHandler(IBackEndApiService api) : IRequestHandler<AddDocumentCommand, GetDocumentResponse>
{
    public Task<GetDocumentResponse> Handle(AddDocumentCommand request, CancellationToken cancellationToken)
    {
        return api.SendRequestAsync<GetDocumentResponse>(new RestRequest(GetDocumentQueryHandler.Route(DocumentRoutes.Add, request.DocumentRequirementSetId), Method.Post).AddJsonBody(request.Document), cancellationToken);
    }
}
public sealed class UpdateDocumentCommandHandler(IBackEndApiService api) : IRequestHandler<UpdateDocumentCommand, GetDocumentResponse>
{
    public Task<GetDocumentResponse> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
    {
        return api.SendRequestAsync<GetDocumentResponse>(new RestRequest(GetDocumentQueryHandler.Route(DocumentRoutes.Requirement, request.DocumentRequirementSetId).Replace("{documentId:guid}", request.DocumentId.ToString()), Method.Put).AddJsonBody(request.Document), cancellationToken);
    }
}
public sealed class DeleteDocumentCommandHandler(IBackEndApiService api) : IRequestHandler<DeleteDocumentCommand, GetDocumentResponse>
{
    public Task<GetDocumentResponse> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        var route = GetDocumentQueryHandler.Route(DocumentRoutes.Requirement, request.DocumentRequirementSetId).Replace("{documentId:guid}", request.DocumentId.ToString());
        return api.SendRequestAsync<GetDocumentResponse>(new RestRequest(route, Method.Delete).AddQueryParameter("rowVersion", request.RowVersion), cancellationToken);
    }
}
