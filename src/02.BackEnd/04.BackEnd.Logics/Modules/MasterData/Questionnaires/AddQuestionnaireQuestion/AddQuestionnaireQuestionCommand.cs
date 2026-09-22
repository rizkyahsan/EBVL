using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.Common;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.AddQuestionnaireQuestion;

[AuthorizeRequestByPermission(QuestionnairePermissions.Manage)]
public sealed record AddQuestionnaireQuestionCommand(Guid QuestionnaireId, Guid SectionId, AddQuestionnaireQuestionRequest Question) : IRequest<GetQuestionnaireResponse>;

public sealed class AddQuestionnaireQuestionCommandValidator : AbstractValidatorBase<AddQuestionnaireQuestionCommand>
{
    public AddQuestionnaireQuestionCommandValidator()
    {
        _ = RuleFor(x => x.Question).SetValidator(new AddQuestionnaireQuestionRequestValidator());
    }
}

public sealed class AddQuestionnaireQuestionHandler(IDatabaseService db) : IRequestHandler<AddQuestionnaireQuestionCommand, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(AddQuestionnaireQuestionCommand r, CancellationToken cancellationToken)
    {
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var questionnaire = await QuestionnaireGraph.Load(db, r.QuestionnaireId, true, cancellationToken);
        QuestionnaireGraph.EnsureDraft(questionnaire);
        await QuestionnaireGraph.EnsureNotReferenced(db, questionnaire.Id, cancellationToken);
        db.SetQuestionnaireOriginalRowVersion(questionnaire, QuestionnaireGraph.ParseRowVersion(r.Question.RowVersion));
        questionnaire.Modified = DateTimeOffset.UtcNow;
        var sourceSection = questionnaire.Sections.SingleOrDefault(section => section.Id == r.SectionId && !section.IsDeleted) ?? throw new KeyNotFoundException("Questionnaire section was not found.");
        var section = questionnaire.Sections.Single(section => section.Code == sourceSection.Code);
        if (questionnaire.Sections.SelectMany(x => x.Questions).Any(x => !x.IsDeleted && x.Code.Equals(r.Question.Code.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new ValidationException("A question with the same code already exists in this questionnaire version.");
        }

        var count = await db.QuestionnaireQuestions.CountAsync(x => x.QuestionnaireSectionId == section.Id && !x.IsDeleted, cancellationToken);
        var order = Math.Min(r.Question.Order, count + 1);
        _ = await db.QuestionnaireQuestions.Where(x => x.QuestionnaireSectionId == section.Id && !x.IsDeleted && x.Order >= order)
            .ExecuteUpdateAsync(update => update.SetProperty(x => x.Order, x => x.Order + 1), cancellationToken);
        var questionId = Guid.CreateVersion7();
        var question = new QuestionnaireQuestion
        {
            Id = questionId,
            QuestionnaireSectionId = section.Id,
            Code = r.Question.Code.Trim(),
            Label = r.Question.Label.Trim(),
            Hint = r.Question.Hint?.Trim(),
            Placeholder = r.Question.Placeholder?.Trim(),
            Type = r.Question.Type,
            CompanyType = r.Question.VendorType,
            Order = order,
            IsRequired = r.Question.AnswerRule == QuestionnaireAnswerRule.Mandatory,
            IsVisible = r.Question.IsVisible,
            IsActive = r.Question.IsActive,
            AnswerRule = r.Question.AnswerRule
        };
        foreach (var option in r.Question.Options.OrderBy(x => x.Order))
        {
            _ = db.QuestionnaireOptions.Add(new QuestionnaireOption { Id = Guid.CreateVersion7(), QuestionnaireQuestionId = questionId, Code = option.Code.Trim(), Label = option.Label.Trim(), Order = option.Order });
        }

        _ = db.QuestionnaireQuestions.Add(question);

        var errors = QuestionnaireGraph.Validate(questionnaire, questionnaire.Rules.Select(QuestionnaireGraph.MapRule).ToList(), false);
        if (errors.Count != 0)
        {
            throw new ValidationException(string.Join("; ", errors));
        }

        await QuestionnaireGraph.SaveMutation(db, nameof(AddQuestionnaireQuestionCommand), cancellationToken);
        await tx.CommitAsync(cancellationToken);
        var q = await QuestionnaireGraph.Load(db, questionnaire.Id, false, cancellationToken);
        return new() { Item = await QuestionnaireGraph.MapWithState(db, q, cancellationToken) };
    }
}
