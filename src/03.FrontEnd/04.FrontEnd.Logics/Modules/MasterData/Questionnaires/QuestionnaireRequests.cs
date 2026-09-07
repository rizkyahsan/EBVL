using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

#pragma warning disable IDE0021, IDE0022, CA1725 // Compact adapters keep route plumbing together.

namespace EBVL.FrontEnd.Logics.Modules.MasterData.Questionnaires;

public sealed record GetQuestionnairesQuery : IRequest<GetQuestionnairesResponse>;
public sealed record GetQuestionnaireQuery(Guid Id) : IRequest<GetQuestionnaireResponse>;
public sealed record AddQuestionnaireCommand : AddQuestionnaireRequest, IRequest<GetQuestionnaireResponse>;
public sealed record AddQuestionnaireQuestionCommand(Guid Id, Guid SectionId, AddQuestionnaireQuestionRequest Question) : IRequest<GetQuestionnaireResponse>;
public sealed record UpdateQuestionnaireDraftCommand(Guid Id, UpdateQuestionnaireDraftRequest Draft) : IRequest<GetQuestionnaireResponse>;
public sealed record ValidateQuestionnaireCommand(Guid Id) : IRequest<ValidateQuestionnaireResponse>;
public sealed record PublishQuestionnaireCommand(Guid Id) : IRequest;
public sealed record DeactivateQuestionnaireCommand(Guid Id) : IRequest;
public sealed record ArchiveQuestionnaireCommand(Guid Id) : IRequest;

public sealed class AddQuestionnaireCommandValidator : AbstractValidatorBase<AddQuestionnaireCommand>
{
    public AddQuestionnaireCommandValidator() => Include(new AddQuestionnaireRequestValidator());
}

public sealed class UpdateQuestionnaireDraftCommandValidator : AbstractValidatorBase<UpdateQuestionnaireDraftCommand>
{
    public UpdateQuestionnaireDraftCommandValidator() => RuleFor(x => x.Draft).SetValidator(new UpdateQuestionnaireDraftRequestValidator());
}

public sealed class GetQuestionnairesQueryHandler(IBackEndApiService api) : IRequestHandler<GetQuestionnairesQuery, GetQuestionnairesResponse>
{
    public Task<GetQuestionnairesResponse> Handle(GetQuestionnairesQuery request, CancellationToken ct) => api.SendRequestAsync<GetQuestionnairesResponse>(new RestRequest(QuestionnaireRoutes.List, Method.Get), ct);
}

public sealed class GetQuestionnaireQueryHandler(IBackEndApiService api) : IRequestHandler<GetQuestionnaireQuery, GetQuestionnaireResponse>
{
    public Task<GetQuestionnaireResponse> Handle(GetQuestionnaireQuery request, CancellationToken ct) => api.SendRequestAsync<GetQuestionnaireResponse>(new RestRequest(QuestionnaireRoutes.Detail.Replace("{id:guid}", request.Id.ToString()), Method.Get), ct);
}

public sealed class AddQuestionnaireCommandHandler(IBackEndApiService api) : IRequestHandler<AddQuestionnaireCommand, GetQuestionnaireResponse>
{
    public Task<GetQuestionnaireResponse> Handle(AddQuestionnaireCommand request, CancellationToken ct)
    {
        var rest = new RestRequest(QuestionnaireRoutes.Add, Method.Post).AddJsonBody(request);
        return api.SendRequestAsync<GetQuestionnaireResponse>(rest, ct);
    }
}
public sealed class AddQuestionnaireQuestionCommandHandler(IBackEndApiService api) : IRequestHandler<AddQuestionnaireQuestionCommand, GetQuestionnaireResponse>
{
    public Task<GetQuestionnaireResponse> Handle(AddQuestionnaireQuestionCommand request, CancellationToken ct)
    {
        var route = QuestionnaireRoutes.AddQuestion.Replace("{id:guid}", request.Id.ToString()).Replace("{sectionId:guid}", request.SectionId.ToString());
        return api.SendRequestAsync<GetQuestionnaireResponse>(new RestRequest(route, Method.Post).AddJsonBody(request.Question), ct);
    }
}

public sealed class UpdateQuestionnaireDraftCommandHandler(IBackEndApiService api) : IRequestHandler<UpdateQuestionnaireDraftCommand, GetQuestionnaireResponse>
{
    public Task<GetQuestionnaireResponse> Handle(UpdateQuestionnaireDraftCommand request, CancellationToken ct)
    {
        var rest = new RestRequest(Uri(QuestionnaireRoutes.Update, request.Id), Method.Put).AddJsonBody(request.Draft);
        return api.SendRequestAsync<GetQuestionnaireResponse>(rest, ct);
    }

    private static string Uri(string route, Guid id) => route.Replace("{id:guid}", id.ToString());
}

public sealed class ValidateQuestionnaireCommandHandler(IBackEndApiService api) : IRequestHandler<ValidateQuestionnaireCommand, ValidateQuestionnaireResponse>
{
    public Task<ValidateQuestionnaireResponse> Handle(ValidateQuestionnaireCommand request, CancellationToken ct) => api.SendRequestAsync<ValidateQuestionnaireResponse>(new RestRequest(QuestionnaireRoutes.Validate.Replace("{id:guid}", request.Id.ToString()), Method.Post), ct);
}

public abstract class QuestionnaireActionHandler<TRequest>(IBackEndApiService api, string route, Method method) : IRequestHandler<TRequest> where TRequest : IRequest
{
    public abstract Task Handle(TRequest request, CancellationToken ct);
    protected Task Send(Guid id, CancellationToken ct) => api.SendRequestAsync(new RestRequest(route.Replace("{id:guid}", id.ToString()), method), ct);
}

public sealed class PublishQuestionnaireCommandHandler(IBackEndApiService api) : QuestionnaireActionHandler<PublishQuestionnaireCommand>(api, QuestionnaireRoutes.Publish, Method.Post) { public override Task Handle(PublishQuestionnaireCommand r, CancellationToken ct) => Send(r.Id, ct); }
public sealed class DeactivateQuestionnaireCommandHandler(IBackEndApiService api) : QuestionnaireActionHandler<DeactivateQuestionnaireCommand>(api, QuestionnaireRoutes.Deactivate, Method.Post) { public override Task Handle(DeactivateQuestionnaireCommand r, CancellationToken ct) => Send(r.Id, ct); }
public sealed class ArchiveQuestionnaireCommandHandler(IBackEndApiService api) : QuestionnaireActionHandler<ArchiveQuestionnaireCommand>(api, QuestionnaireRoutes.Archive, Method.Post) { public override Task Handle(ArchiveQuestionnaireCommand r, CancellationToken ct) => Send(r.Id, ct); }
