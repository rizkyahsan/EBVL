using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.Common;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.UpdateQuestionnaireQuestion;

[AuthorizeRequestByPermission(QuestionnairePermissions.Manage)]
public sealed record UpdateQuestionnaireQuestionCommand(Guid QuestionnaireId, Guid SectionId, Guid QuestionId, UpdateQuestionnaireQuestionRequest Question) : IRequest<GetQuestionnaireResponse>;

public sealed class UpdateQuestionnaireQuestionCommandValidator : AbstractValidatorBase<UpdateQuestionnaireQuestionCommand>
{
    public UpdateQuestionnaireQuestionCommandValidator()
    {
        _ = RuleFor(x => x.Question).SetValidator(new UpdateQuestionnaireQuestionRequestValidator());
    }
}

public sealed class UpdateQuestionnaireQuestionHandler(IDatabaseService db) : IRequestHandler<UpdateQuestionnaireQuestionCommand, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(UpdateQuestionnaireQuestionCommand r, CancellationToken cancellationToken)
    {
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var q = await QuestionnaireGraph.Load(db, r.QuestionnaireId, true, cancellationToken);
        QuestionnaireGraph.EnsureDraft(q);
        await QuestionnaireGraph.EnsureNotReferenced(db, q.Id, cancellationToken);
        db.SetQuestionnaireOriginalRowVersion(q, QuestionnaireGraph.ParseRowVersion(r.Question.RowVersion));
        var section = q.Sections.SingleOrDefault(x => x.Id == r.SectionId && !x.IsDeleted) ?? throw new KeyNotFoundException("Questionnaire section was not found.");
        var question = section.Questions.SingleOrDefault(x => x.Id == r.QuestionId && !x.IsDeleted) ?? throw new KeyNotFoundException("Questionnaire question was not found.");
        var code = r.Question.Code.Trim();
        if (q.Sections.SelectMany(x => x.Questions).Any(x => x.Id != question.Id && !x.IsDeleted && x.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ValidationException("A question with the same code already exists in this questionnaire version.");
        }

        var requestedOrder = Math.Clamp(r.Question.Order, 1, section.Questions.Count);
        foreach (var item in section.Questions.Where(x => x.Id != question.Id).OrderBy(x => x.Order))
        {
            item.Order = item.Order < requestedOrder ? item.Order : item.Order + 1;
        }

        question.Code = code;
        question.Label = r.Question.Label.Trim();
        question.Hint = r.Question.Hint?.Trim();
        question.Placeholder = r.Question.Placeholder?.Trim();
        question.Type = r.Question.Type;
        question.CompanyType = r.Question.VendorType;
        question.Order = requestedOrder;
        question.AnswerRule = r.Question.AnswerRule;
        question.IsRequired = r.Question.AnswerRule == QuestionnaireAnswerRule.Mandatory;
        question.IsVisible = r.Question.IsVisible;
        question.IsActive = r.Question.IsActive;
        db.QuestionnaireOptions.RemoveRange(question.Options);
        question.Options.Clear();
        foreach (var option in r.Question.Options.OrderBy(x => x.Order))
        {
            question.Options.Add(new QuestionnaireOption { QuestionnaireQuestionId = question.Id, Code = option.Code.Trim(), Label = option.Label.Trim(), Order = option.Order });
        }

        var order = 1;
        foreach (var item in section.Questions.OrderBy(x => x.Order).ThenBy(x => x.Id))
        {
            item.Order = order++;
        }

        var errors = QuestionnaireGraph.Validate(q, q.Rules.Select(QuestionnaireGraph.MapRule).ToList(), false);
        if (errors.Count != 0)
        {
            throw new ValidationException(string.Join("; ", errors));
        }

        q.Modified = DateTimeOffset.UtcNow;
        await QuestionnaireGraph.SaveMutation(db, nameof(UpdateQuestionnaireQuestionCommand), cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return new() { Item = await QuestionnaireGraph.MapWithState(db, q, cancellationToken) };
    }
}
