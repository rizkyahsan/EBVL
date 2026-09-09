using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires;

// TODO: Restore questionnaire permissions after IdAMan permission setup is complete.
public sealed record GetQuestionnairesQuery : IRequest<GetQuestionnairesResponse>;
public sealed record GetQuestionnaireQuery(Guid QuestionnaireId) : IRequest<GetQuestionnaireResponse>;
public sealed record AddQuestionnaireCommand : AddQuestionnaireRequest, IRequest<GetQuestionnaireResponse>;
public sealed record AddQuestionnaireQuestionCommand(Guid QuestionnaireId, Guid SectionId, AddQuestionnaireQuestionRequest Question) : IRequest<GetQuestionnaireResponse>;
public sealed record UpdateQuestionnaireCommand(Guid QuestionnaireId, UpdateQuestionnaireRequest Questionnaire) : IRequest<GetQuestionnaireResponse>;

public sealed class AddQuestionnaireCommandValidator : AbstractValidatorBase<AddQuestionnaireCommand>
{
    public AddQuestionnaireCommandValidator()
    {
        Include(new AddQuestionnaireRequestValidator());
    }
}
public sealed class AddQuestionnaireQuestionCommandValidator : AbstractValidatorBase<AddQuestionnaireQuestionCommand>
{
    public AddQuestionnaireQuestionCommandValidator()
    {
        _ = RuleFor(x => x.Question).SetValidator(new AddQuestionnaireQuestionRequestValidator());
    }
}
public sealed class UpdateQuestionnaireCommandValidator : AbstractValidatorBase<UpdateQuestionnaireCommand>
{
    public UpdateQuestionnaireCommandValidator()
    {
        _ = RuleFor(x => x.Questionnaire).SetValidator(new UpdateQuestionnaireRequestValidator());
    }
}

public sealed class GetQuestionnairesHandler(IDatabaseService db) : IRequestHandler<GetQuestionnairesQuery, GetQuestionnairesResponse>
{
    public async Task<GetQuestionnairesResponse> Handle(GetQuestionnairesQuery r, CancellationToken cancellationToken)
    {
        var questionnaires = await db.Questionnaires.AsNoTracking().Include(x => x.Sections).Where(x => !x.IsDeleted).ToListAsync(cancellationToken);
        var rows = questionnaires.SelectMany(q => q.Sections.Where(s => !s.IsDeleted), (q, s) => new QuestionnaireListItem(q.Id, s.Id, q.Code, q.BusinessProcess, s.CompanyType, s.Title, q.IsActive, s.IsActive)).OrderBy(x => x.BusinessProcess).ThenBy(x => x.Section).ToList();
        return new() { Items = rows };
    }
}
public sealed class GetQuestionnaireHandler(IDatabaseService db) : IRequestHandler<GetQuestionnaireQuery, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(GetQuestionnaireQuery r, CancellationToken cancellationToken)
    {
        var q = await QuestionnaireGraph.Load(db, r.QuestionnaireId, false, cancellationToken);
        return new() { Item = QuestionnaireGraph.Map(q) };
    }
}
public sealed class AddQuestionnaireHandler(IDatabaseService db) : IRequestHandler<AddQuestionnaireCommand, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(AddQuestionnaireCommand r, CancellationToken cancellationToken)
    {
        var code = Code(r.BusinessProcess);
        var questionnaire = await db.Questionnaires.Include(x => x.Sections).ThenInclude(x => x.Questions).ThenInclude(x => x.Options).Include(x => x.Rules).SingleOrDefaultAsync(x => x.Code == code && !x.IsDeleted, cancellationToken);
        if (questionnaire is null)
        {
            questionnaire = new Questionnaire { Code = code, BusinessProcess = r.BusinessProcess.Trim(), IsActive = r.IsActive };
            _ = await db.Questionnaires.AddAsync(questionnaire, cancellationToken);
        }

        var baseCode = Code(r.Section);
        var sectionCode = baseCode;
        for (var suffix = 2; questionnaire.Sections.Any(x => x.Code == sectionCode); suffix++)
        {
            sectionCode = $"{baseCode}_{suffix}";
        }

        questionnaire.Sections.Add(new QuestionnaireSection { Code = sectionCode, Title = r.Section.Trim(), Order = questionnaire.Sections.Count + 1, CompanyType = r.VendorType, IsActive = r.IsActive });
        _ = await db.SaveAsync(nameof(AddQuestionnaireCommand), cancellationToken);
        return new() { Item = QuestionnaireGraph.Map(questionnaire) };
    }
    private static string Code(string value)
    {
        return string.Join('_', value.Trim().ToUpperInvariant().Split([' ', '-', '/'], StringSplitOptions.RemoveEmptyEntries));
    }
}
public sealed class UpdateQuestionnaireHandler(IDatabaseService db) : IRequestHandler<UpdateQuestionnaireCommand, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(UpdateQuestionnaireCommand r, CancellationToken cancellationToken)
    {
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var q = await QuestionnaireGraph.Load(db, r.QuestionnaireId, true, cancellationToken);
        db.SetQuestionnaireOriginalRowVersion(q, Convert.FromBase64String(r.Questionnaire.RowVersion));
        q.BusinessProcess = r.Questionnaire.BusinessProcess.Trim();
        q.IsActive = r.Questionnaire.IsActive;
        db.QuestionnaireRules.RemoveRange(await db.QuestionnaireRules.Where(x => x.QuestionnaireId == q.Id).ToListAsync(cancellationToken));
        db.QuestionnaireSections.RemoveRange(q.Sections);
        q.Sections.Clear();
        foreach (var s in r.Questionnaire.Sections.OrderBy(x => x.Order))
        {
            var section = new QuestionnaireSection { QuestionnaireId = q.Id, Code = s.Code, Title = s.Title, Order = s.Order, CompanyType = s.CompanyType, IsActive = s.IsActive };
            foreach (var item in s.Questions.OrderBy(x => x.Order))
            {
                var question = new QuestionnaireQuestion { Id = item.Id == Guid.Empty ? Guid.CreateVersion7() : item.Id, Code = item.Code, Label = item.Label, Hint = item.Hint, Placeholder = item.Placeholder, Type = item.Type, CompanyType = item.CompanyType, Order = item.Order, IsRequired = item.AnswerRule == QuestionnaireAnswerRule.Mandatory, IsVisible = item.IsVisible, IsActive = item.IsActive, AnswerRule = item.AnswerRule };
                foreach (var option in item.Options.OrderBy(x => x.Order))
                {
                    question.Options.Add(new QuestionnaireOption { Code = option.Code, Label = option.Label, Order = option.Order });
                }

                section.Questions.Add(question);
            }

            q.Sections.Add(section);
        }

        foreach (var rule in r.Questionnaire.Rules)
        {
            _ = await db.QuestionnaireRules.AddAsync(new QuestionnaireRule { QuestionnaireId = q.Id, SourceQuestionId = rule.SourceQuestionId, TargetQuestionId = rule.TargetQuestionId, Operator = rule.Operator, Action = rule.Action, Value = rule.Value }, cancellationToken);
        }

        var errors = QuestionnaireGraph.Validate(q, r.Questionnaire.Rules);
        if (errors.Count != 0)
        {
            throw new ValidationException(string.Join("; ", errors));
        }

        _ = await db.SaveAsync(nameof(UpdateQuestionnaireCommand), cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return new() { Item = QuestionnaireGraph.Map(q, r.Questionnaire.Rules) };
    }
}
public sealed class AddQuestionnaireQuestionHandler(IDatabaseService db) : IRequestHandler<AddQuestionnaireQuestionCommand, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(AddQuestionnaireQuestionCommand r, CancellationToken cancellationToken)
    {
        var section = await db.QuestionnaireSections.SingleOrDefaultAsync(x => x.Id == r.SectionId && x.QuestionnaireId == r.QuestionnaireId && !x.IsDeleted, cancellationToken) ?? throw new KeyNotFoundException("Questionnaire section was not found.");
        var count = await db.QuestionnaireQuestions.CountAsync(x => x.QuestionnaireSectionId == section.Id && !x.IsDeleted, cancellationToken);
        var order = Math.Min(r.Question.Order, count + 1);
        _ = await db.QuestionnaireQuestions.Where(x => x.QuestionnaireSectionId == section.Id && !x.IsDeleted && x.Order >= order)
            .ExecuteUpdateAsync(update => update.SetProperty(x => x.Order, x => x.Order + 1), cancellationToken);
        section.Questions.Add(new QuestionnaireQuestion { Code = r.Question.Code.Trim(), Label = r.Question.Name.Trim(), Hint = r.Question.Description?.Trim(), Type = r.Question.AnswerType, CompanyType = r.Question.VendorType, Order = order, IsRequired = r.Question.AnswerRule == QuestionnaireAnswerRule.Mandatory, IsVisible = true, IsActive = r.Question.IsActive, AnswerRule = r.Question.AnswerRule });
        _ = await db.SaveAsync(nameof(AddQuestionnaireQuestionCommand), cancellationToken);
        var q = await QuestionnaireGraph.Load(db, r.QuestionnaireId, false, cancellationToken);
        return new() { Item = QuestionnaireGraph.Map(q) };
    }
}
internal static class QuestionnaireGraph
{
    public static async Task<Questionnaire> Load(IDatabaseService db, Guid id, bool tracking, CancellationToken ct)
    {
        var query = db.Questionnaires.Include(x => x.Sections).ThenInclude(x => x.Questions).ThenInclude(x => x.Options).Include(x => x.Rules).Where(x => x.Id == id && !x.IsDeleted);
        if (!tracking)
        {
            query = query.AsNoTracking();
        }

        return await query.SingleOrDefaultAsync(ct) ?? throw new InvalidOperationException("Questionnaire was not found.");
    }
    public static List<string> Validate(Questionnaire q, IReadOnlyList<QuestionnaireRuleItem> rules)
    {
        var errors = new List<string>();
        var questions = q.Sections.SelectMany(x => x.Questions).ToList();
        if (q.Sections.Count == 0)
        {
            errors.Add("At least one section is required.");
        }

        if (questions.Count == 0)
        {
            errors.Add("At least one question is required.");
        }

        foreach (var g in q.Sections.GroupBy(x => x.Code, StringComparer.OrdinalIgnoreCase).Where(x => x.Count() > 1))
        {
            errors.Add($"Duplicate section code: {g.Key}.");
        }

        foreach (var g in questions.GroupBy(x => x.Code, StringComparer.OrdinalIgnoreCase).Where(x => x.Count() > 1))
        {
            errors.Add($"Duplicate question code: {g.Key}.");
        }

        foreach (var item in questions)
        {
            var choice = item.Type is QuestionnaireQuestionType.SingleChoice or QuestionnaireQuestionType.MultipleChoice;
            if (choice && item.Options.Count == 0)
            {
                errors.Add($"{item.Code} requires options.");
            }

            if (!choice && item.Options.Count != 0)
            {
                errors.Add($"{item.Code} type does not support options.");
            }
        }

        var ids = questions.Select(x => x.Id).ToHashSet();
        foreach (var rule in rules)
        {
            if (!ids.Contains(rule.SourceQuestionId) || !ids.Contains(rule.TargetQuestionId))
            {
                errors.Add("Rules must reference questions in the same questionnaire.");
            }
        }

        return errors;
    }
    public static QuestionnaireItem Map(Questionnaire q, IReadOnlyList<QuestionnaireRuleItem>? rules = null)
    {
        return new(q.Id, q.Code, q.BusinessProcess, q.IsActive, Convert.ToBase64String(q.RowVersion), q.Sections.OrderBy(x => x.Order).Select(s => new QuestionnaireSectionItem(s.Id, s.Code, s.Title, s.Order, s.CompanyType, s.IsActive, s.Questions.OrderBy(x => x.Order).Select(item => new QuestionnaireQuestionItem(item.Id, item.Code, item.Label, item.Hint, item.Placeholder, item.Type, item.CompanyType, item.Order, item.IsRequired, item.IsVisible, item.IsActive, item.AnswerRule, item.Options.OrderBy(x => x.Order).Select(o => new QuestionnaireOptionItem(o.Id, o.Code, o.Label, o.Order)).ToList())).ToList())).ToList(), rules ?? q.Rules.Select(x => new QuestionnaireRuleItem(x.Id, x.SourceQuestionId, x.TargetQuestionId, x.Operator, x.Action, x.Value)).ToList());
    }
}
