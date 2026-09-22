using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.Common;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.GetQuestionnaire;

[AuthorizeRequest]
public sealed record GetQuestionnaireQuery(Guid QuestionnaireId) : IRequest<GetQuestionnaireResponse>;

public sealed class GetQuestionnaireHandler(IDatabaseService db) : IRequestHandler<GetQuestionnaireQuery, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(GetQuestionnaireQuery r, CancellationToken cancellationToken)
    {
        var q = await QuestionnaireGraph.Load(db, r.QuestionnaireId, false, cancellationToken);
        return new() { Item = await QuestionnaireGraph.MapWithState(db, q, cancellationToken) };
    }
}
