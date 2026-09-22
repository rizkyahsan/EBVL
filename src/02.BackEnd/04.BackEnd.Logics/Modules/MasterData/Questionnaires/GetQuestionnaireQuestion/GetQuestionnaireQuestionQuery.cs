using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.Common;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.GetQuestionnaireQuestion;

[AuthorizeRequest]
public sealed record GetQuestionnaireQuestionQuery(Guid QuestionnaireId, Guid SectionId, Guid QuestionId) : IRequest<GetQuestionnaireQuestionResponse>;

public sealed class GetQuestionnaireQuestionHandler(IDatabaseService db) : IRequestHandler<GetQuestionnaireQuestionQuery, GetQuestionnaireQuestionResponse>
{
    public async Task<GetQuestionnaireQuestionResponse> Handle(GetQuestionnaireQuestionQuery r, CancellationToken cancellationToken)
    {
        var section = await QuestionnaireGraph.LoadSection(db, r.QuestionnaireId, r.SectionId, false, cancellationToken);
        var question = section.Questions.SingleOrDefault(x => x.Id == r.QuestionId && !x.IsDeleted) ?? throw new KeyNotFoundException("Questionnaire question was not found.");
        return new() { Item = QuestionnaireGraph.MapQuestion(question) };
    }
}
