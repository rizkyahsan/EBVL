using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.Common;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.UpdateQuestionnaire;

[AuthorizeRequestByPermission(QuestionnairePermissions.Manage)]
public sealed record UpdateQuestionnaireCommand(Guid QuestionnaireId, UpdateQuestionnaireRequest Questionnaire) : IRequest<GetQuestionnaireResponse>;

public sealed class UpdateQuestionnaireCommandValidator : AbstractValidatorBase<UpdateQuestionnaireCommand>
{
    public UpdateQuestionnaireCommandValidator()
    {
        _ = RuleFor(x => x.Questionnaire).SetValidator(new UpdateQuestionnaireRequestValidator());
    }
}

public sealed class UpdateQuestionnaireHandler(IDatabaseService db) : IRequestHandler<UpdateQuestionnaireCommand, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(UpdateQuestionnaireCommand r, CancellationToken cancellationToken)
    {
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var q = await QuestionnaireGraph.Load(db, r.QuestionnaireId, true, cancellationToken);
        QuestionnaireGraph.EnsureDraft(q);
        if (await QuestionnaireGraph.IsReferenced(db, q.Id, cancellationToken))
        {
            throw new ValidationException("A questionnaire draft used by a registration cannot be changed.");
        }

        db.SetQuestionnaireOriginalRowVersion(q, QuestionnaireGraph.ParseRowVersion(r.Questionnaire.RowVersion));
        q.BusinessProcess = r.Questionnaire.BusinessProcess.Trim();
        q.IsActive = r.Questionnaire.IsActive;
        q.Modified = DateTimeOffset.UtcNow;
        db.QuestionnaireRules.RemoveRange(q.Rules);
        q.Rules.Clear();
        var questionIds = new Dictionary<Guid, Guid>();
        var retainedSectionIds = new HashSet<Guid>();
        foreach (var s in r.Questionnaire.Sections.OrderBy(x => x.Order))
        {
            var section = q.Sections.SingleOrDefault(x => x.Id == s.Id);
            if (section is null)
            {
                section = new QuestionnaireSection { Id = Guid.CreateVersion7(), QuestionnaireId = q.Id, Code = s.Code, Title = s.Title };
                q.Sections.Add(section);
            }

            _ = retainedSectionIds.Add(section.Id);
            section.Code = s.Code;
            section.Title = s.Title;
            section.Order = s.Order;
            section.CompanyType = s.CompanyType;
            section.IsActive = s.IsActive;
            var retainedQuestionIds = new HashSet<Guid>();
            foreach (var item in s.Questions.OrderBy(x => x.Order))
            {
                var question = section.Questions.SingleOrDefault(x => x.Id == item.Id);
                if (question is null)
                {
                    question = new QuestionnaireQuestion { Id = Guid.CreateVersion7(), QuestionnaireSectionId = section.Id, Code = item.Code, Label = item.Label };
                    section.Questions.Add(question);
                }

                _ = retainedQuestionIds.Add(question.Id);
                questionIds[item.Id] = question.Id;
                question.Code = item.Code;
                question.Label = item.Label;
                question.Hint = item.Hint;
                question.Placeholder = item.Placeholder;
                question.Type = item.Type;
                question.CompanyType = item.CompanyType;
                question.Order = item.Order;
                question.IsRequired = item.AnswerRule == QuestionnaireAnswerRule.Mandatory;
                question.IsVisible = item.IsVisible;
                question.IsActive = item.IsActive;
                question.AnswerRule = item.AnswerRule;
                db.QuestionnaireOptions.RemoveRange(question.Options);
                question.Options.Clear();
                foreach (var option in item.Options.OrderBy(x => x.Order))
                {
                    question.Options.Add(new QuestionnaireOption { Id = Guid.CreateVersion7(), QuestionnaireQuestionId = question.Id, Code = option.Code, Label = option.Label, Order = option.Order });
                }
            }

            var removedQuestions = section.Questions.Where(x => !retainedQuestionIds.Contains(x.Id)).ToList();
            db.QuestionnaireQuestions.RemoveRange(removedQuestions);
            foreach (var removedQuestion in removedQuestions)
            {
                _ = section.Questions.Remove(removedQuestion);
            }
        }

        var removedSections = q.Sections.Where(x => !retainedSectionIds.Contains(x.Id)).ToList();
        db.QuestionnaireSections.RemoveRange(removedSections);
        foreach (var removedSection in removedSections)
        {
            _ = q.Sections.Remove(removedSection);
        }

        var mappedRules = new List<QuestionnaireRuleItem>();
        foreach (var rule in r.Questionnaire.Rules)
        {
            if (!questionIds.TryGetValue(rule.SourceQuestionId, out var sourceQuestionId)
                || !questionIds.TryGetValue(rule.TargetQuestionId, out var targetQuestionId))
            {
                throw new ValidationException("Rules must reference questions in the same questionnaire.");
            }

            var mappedRule = new QuestionnaireRuleItem(Guid.CreateVersion7(), sourceQuestionId, targetQuestionId, rule.Operator, rule.Action, rule.Value);
            mappedRules.Add(mappedRule);
            q.Rules.Add(new QuestionnaireRule { Id = mappedRule.Id, QuestionnaireId = q.Id, SourceQuestionId = sourceQuestionId, TargetQuestionId = targetQuestionId, Operator = rule.Operator, Action = rule.Action, Value = rule.Value });
        }

        var errors = QuestionnaireGraph.Validate(q, mappedRules, false);
        if (errors.Count != 0)
        {
            throw new ValidationException(string.Join("; ", errors));
        }

        try
        {
            _ = await db.SaveAsync(nameof(UpdateQuestionnaireCommand), cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ValidationException("The questionnaire has changed since this page was loaded. Reload the page and try again.");
        }

        await tx.CommitAsync(cancellationToken);
        return new() { Item = await QuestionnaireGraph.MapWithState(db, q, cancellationToken, mappedRules) };
    }
}
