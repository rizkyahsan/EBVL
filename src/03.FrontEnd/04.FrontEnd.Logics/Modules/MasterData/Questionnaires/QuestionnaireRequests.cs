using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

#pragma warning disable IDE0021, IDE0022, CA1725
namespace EBVL.FrontEnd.Logics.Modules.MasterData.Questionnaires;

#region Requests

public sealed record GetQuestionnairesQuery : IRequest<GetQuestionnairesResponse>;
public sealed record GetQuestionnaireQuery(Guid QuestionnaireId) : IRequest<GetQuestionnaireResponse>;
public sealed record GetQuestionnaireVersionHistoryQuery(Guid QuestionnaireSeriesId) : IRequest<GetQuestionnaireVersionHistoryResponse>;
public sealed record GetQuestionnaireSectionsQuery(Guid QuestionnaireId) : GetQuestionnaireSectionsRequest, IRequest<GetQuestionnaireSectionsResponse>;
public sealed record GetQuestionnaireQuestionsQuery(Guid QuestionnaireId, Guid SectionId) : GetQuestionnaireQuestionsRequest, IRequest<GetQuestionnaireQuestionsResponse>;
public sealed record GetQuestionnaireQuestionQuery(Guid QuestionnaireId, Guid SectionId, Guid QuestionId) : IRequest<GetQuestionnaireQuestionResponse>;
public sealed record AddQuestionnaireCommand : AddQuestionnaireRequest, IRequest<GetQuestionnaireResponse>;
public sealed record CreateQuestionnaireDraftCommand(Guid QuestionnaireId) : IRequest<GetQuestionnaireResponse>;
public sealed record PublishQuestionnaireCommand(Guid QuestionnaireId, string RowVersion) : IRequest<GetQuestionnaireResponse>;
public sealed record AddQuestionnaireSectionCommand(Guid QuestionnaireId, AddQuestionnaireSectionRequest Section) : IRequest<GetQuestionnaireResponse>;
public sealed record AddQuestionnaireQuestionCommand(Guid QuestionnaireId, Guid SectionId, AddQuestionnaireQuestionRequest Question) : IRequest<GetQuestionnaireResponse>;
public sealed record UpdateQuestionnaireSectionCommand(Guid QuestionnaireId, Guid SectionId, UpdateQuestionnaireSectionRequest Section) : IRequest<GetQuestionnaireResponse>;
public sealed record DeleteQuestionnaireSectionCommand(Guid QuestionnaireId, Guid SectionId, string RowVersion) : IRequest<GetQuestionnaireResponse>;
public sealed record UpdateQuestionnaireQuestionCommand(Guid QuestionnaireId, Guid SectionId, Guid QuestionId, UpdateQuestionnaireQuestionRequest Question) : IRequest<GetQuestionnaireResponse>;
public sealed record DeleteQuestionnaireQuestionCommand(Guid QuestionnaireId, Guid SectionId, Guid QuestionId, string RowVersion) : IRequest<GetQuestionnaireResponse>;
public sealed record UpdateQuestionnaireCommand(Guid QuestionnaireId, UpdateQuestionnaireRequest Questionnaire) : IRequest<GetQuestionnaireResponse>;

#endregion

#region Validation

public sealed class AddQuestionnaireCommandValidator : AbstractValidatorBase<AddQuestionnaireCommand> { public AddQuestionnaireCommandValidator() => Include(new AddQuestionnaireRequestValidator()); }
public sealed class AddQuestionnaireQuestionCommandValidator : AbstractValidatorBase<AddQuestionnaireQuestionCommand> { public AddQuestionnaireQuestionCommandValidator() => RuleFor(x => x.Question).SetValidator(new AddQuestionnaireQuestionRequestValidator()); }
public sealed class UpdateQuestionnaireCommandValidator : AbstractValidatorBase<UpdateQuestionnaireCommand> { public UpdateQuestionnaireCommandValidator() => RuleFor(x => x.Questionnaire).SetValidator(new UpdateQuestionnaireRequestValidator()); }

#endregion

#region Query Handlers

public sealed class GetQuestionnairesQueryHandler(IBackEndApiService api) : IRequestHandler<GetQuestionnairesQuery, GetQuestionnairesResponse> { public Task<GetQuestionnairesResponse> Handle(GetQuestionnairesQuery r, CancellationToken ct) => api.SendRequestAsync<GetQuestionnairesResponse>(new RestRequest(GetQuestionnairesRoute.Pattern, Method.Get), ct); }
public sealed class GetQuestionnaireQueryHandler(IBackEndApiService api) : IRequestHandler<GetQuestionnaireQuery, GetQuestionnaireResponse> { public Task<GetQuestionnaireResponse> Handle(GetQuestionnaireQuery r, CancellationToken ct) => api.SendRequestAsync<GetQuestionnaireResponse>(new RestRequest(GetQuestionnaireRoute.ResourceUri(r.QuestionnaireId), Method.Get), ct); }
public sealed class GetQuestionnaireVersionHistoryQueryHandler(IBackEndApiService api) : IRequestHandler<GetQuestionnaireVersionHistoryQuery, GetQuestionnaireVersionHistoryResponse> { public Task<GetQuestionnaireVersionHistoryResponse> Handle(GetQuestionnaireVersionHistoryQuery r, CancellationToken ct) => api.SendRequestAsync<GetQuestionnaireVersionHistoryResponse>(new RestRequest(GetQuestionnaireVersionHistoryRoute.ResourceUri(r.QuestionnaireSeriesId), Method.Get), ct); }
public sealed class GetQuestionnaireSectionsQueryHandler(IBackEndApiService api) : IRequestHandler<GetQuestionnaireSectionsQuery, GetQuestionnaireSectionsResponse> { public Task<GetQuestionnaireSectionsResponse> Handle(GetQuestionnaireSectionsQuery r, CancellationToken ct) => api.SendRequestAsync<GetQuestionnaireSectionsResponse>(Request(GetQuestionnaireSectionsRoute.ResourceUri(r.QuestionnaireId), r), ct); private static RestRequest Request(string route, GetQuestionnaireSectionsRequest r) => new RestRequest(route, Method.Get).AddQueryParameter("page", r.Page?.ToString()).AddQueryParameter("pageSize", r.PageSize?.ToString()).AddQueryParameter("searchText", r.SearchText).AddQueryParameter("sortField", r.SortField).AddQueryParameter("sortOrder", r.SortOrder?.ToString()); }
public sealed class GetQuestionnaireQuestionsQueryHandler(IBackEndApiService api) : IRequestHandler<GetQuestionnaireQuestionsQuery, GetQuestionnaireQuestionsResponse> { public Task<GetQuestionnaireQuestionsResponse> Handle(GetQuestionnaireQuestionsQuery r, CancellationToken ct) => api.SendRequestAsync<GetQuestionnaireQuestionsResponse>(Request(GetQuestionnaireQuestionsRoute.ResourceUri(r.QuestionnaireId, r.SectionId), r), ct); private static RestRequest Request(string route, GetQuestionnaireQuestionsRequest r) => new RestRequest(route, Method.Get).AddQueryParameter("page", r.Page?.ToString()).AddQueryParameter("pageSize", r.PageSize?.ToString()).AddQueryParameter("searchText", r.SearchText).AddQueryParameter("sortField", r.SortField).AddQueryParameter("sortOrder", r.SortOrder?.ToString()); }
public sealed class GetQuestionnaireQuestionQueryHandler(IBackEndApiService api) : IRequestHandler<GetQuestionnaireQuestionQuery, GetQuestionnaireQuestionResponse> { public Task<GetQuestionnaireQuestionResponse> Handle(GetQuestionnaireQuestionQuery r, CancellationToken ct) => api.SendRequestAsync<GetQuestionnaireQuestionResponse>(new RestRequest(GetQuestionnaireQuestionRoute.ResourceUri(r.QuestionnaireId, r.SectionId, r.QuestionId), Method.Get), ct); }

#endregion

#region Command Handlers

public sealed class AddQuestionnaireCommandHandler(IBackEndApiService api) : IRequestHandler<AddQuestionnaireCommand, GetQuestionnaireResponse> { public Task<GetQuestionnaireResponse> Handle(AddQuestionnaireCommand r, CancellationToken ct) => api.SendRequestAsync<GetQuestionnaireResponse>(new RestRequest(AddQuestionnaireRoute.Pattern, Method.Post).AddJsonBody(r), ct); }
public sealed class CreateQuestionnaireDraftCommandHandler(IBackEndApiService api) : IRequestHandler<CreateQuestionnaireDraftCommand, GetQuestionnaireResponse> { public Task<GetQuestionnaireResponse> Handle(CreateQuestionnaireDraftCommand r, CancellationToken ct) => api.SendRequestAsync<GetQuestionnaireResponse>(new RestRequest(CreateQuestionnaireDraftRoute.ResourceUri(r.QuestionnaireId), Method.Post), ct); }
public sealed class PublishQuestionnaireCommandHandler(IBackEndApiService api) : IRequestHandler<PublishQuestionnaireCommand, GetQuestionnaireResponse> { public Task<GetQuestionnaireResponse> Handle(PublishQuestionnaireCommand r, CancellationToken ct) => api.SendRequestAsync<GetQuestionnaireResponse>(new RestRequest(PublishQuestionnaireRoute.ResourceUri(r.QuestionnaireId), Method.Post).AddJsonBody(new PublishQuestionnaireRequest { RowVersion = r.RowVersion }), ct); }
public sealed class AddQuestionnaireSectionCommandHandler(IBackEndApiService api) : IRequestHandler<AddQuestionnaireSectionCommand, GetQuestionnaireResponse> { public Task<GetQuestionnaireResponse> Handle(AddQuestionnaireSectionCommand r, CancellationToken ct) => api.SendRequestAsync<GetQuestionnaireResponse>(new RestRequest(AddQuestionnaireSectionRoute.ResourceUri(r.QuestionnaireId), Method.Post).AddJsonBody(r.Section), ct); }
public sealed class AddQuestionnaireQuestionCommandHandler(IBackEndApiService api) : IRequestHandler<AddQuestionnaireQuestionCommand, GetQuestionnaireResponse> { public Task<GetQuestionnaireResponse> Handle(AddQuestionnaireQuestionCommand r, CancellationToken ct) => api.SendRequestAsync<GetQuestionnaireResponse>(new RestRequest(AddQuestionnaireQuestionRoute.ResourceUri(r.QuestionnaireId, r.SectionId), Method.Post).AddJsonBody(r.Question), ct); }
public sealed class UpdateQuestionnaireSectionCommandHandler(IBackEndApiService api) : IRequestHandler<UpdateQuestionnaireSectionCommand, GetQuestionnaireResponse> { public Task<GetQuestionnaireResponse> Handle(UpdateQuestionnaireSectionCommand r, CancellationToken ct) => api.SendRequestAsync<GetQuestionnaireResponse>(new RestRequest(UpdateQuestionnaireSectionRoute.ResourceUri(r.QuestionnaireId, r.SectionId), Method.Put).AddJsonBody(r.Section), ct); }
public sealed class DeleteQuestionnaireSectionCommandHandler(IBackEndApiService api) : IRequestHandler<DeleteQuestionnaireSectionCommand, GetQuestionnaireResponse> { public Task<GetQuestionnaireResponse> Handle(DeleteQuestionnaireSectionCommand r, CancellationToken ct) => api.SendRequestAsync<GetQuestionnaireResponse>(new RestRequest(DeleteQuestionnaireSectionRoute.ResourceUri(r.QuestionnaireId, r.SectionId), Method.Delete).AddQueryParameter("rowVersion", r.RowVersion), ct); }
public sealed class UpdateQuestionnaireQuestionCommandHandler(IBackEndApiService api) : IRequestHandler<UpdateQuestionnaireQuestionCommand, GetQuestionnaireResponse> { public Task<GetQuestionnaireResponse> Handle(UpdateQuestionnaireQuestionCommand r, CancellationToken ct) => api.SendRequestAsync<GetQuestionnaireResponse>(new RestRequest(UpdateQuestionnaireQuestionRoute.ResourceUri(r.QuestionnaireId, r.SectionId, r.QuestionId), Method.Put).AddJsonBody(r.Question), ct); }
public sealed class DeleteQuestionnaireQuestionCommandHandler(IBackEndApiService api) : IRequestHandler<DeleteQuestionnaireQuestionCommand, GetQuestionnaireResponse> { public Task<GetQuestionnaireResponse> Handle(DeleteQuestionnaireQuestionCommand r, CancellationToken ct) => api.SendRequestAsync<GetQuestionnaireResponse>(new RestRequest(DeleteQuestionnaireQuestionRoute.ResourceUri(r.QuestionnaireId, r.SectionId, r.QuestionId), Method.Delete).AddQueryParameter("rowVersion", r.RowVersion), ct); }
public sealed class UpdateQuestionnaireCommandHandler(IBackEndApiService api) : IRequestHandler<UpdateQuestionnaireCommand, GetQuestionnaireResponse> { public Task<GetQuestionnaireResponse> Handle(UpdateQuestionnaireCommand r, CancellationToken ct) => api.SendRequestAsync<GetQuestionnaireResponse>(new RestRequest(UpdateQuestionnaireRoute.ResourceUri(r.QuestionnaireId), Method.Put).AddJsonBody(r.Questionnaire), ct); }

#endregion
