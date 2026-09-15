using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires;

// TODO: Restore questionnaire permissions after IdAMan permission setup is complete.
#region Requests

public sealed record GetQuestionnairesQuery : IRequest<GetQuestionnairesResponse>;
public sealed record GetQuestionnaireQuery(Guid QuestionnaireId) : IRequest<GetQuestionnaireResponse>;
public sealed record AddQuestionnaireCommand : AddQuestionnaireRequest, IRequest<GetQuestionnaireResponse>;
public sealed record AddQuestionnaireQuestionCommand(Guid QuestionnaireId, Guid SectionId, AddQuestionnaireQuestionRequest Question) : IRequest<GetQuestionnaireResponse>;
public sealed record UpdateQuestionnaireSectionCommand(Guid QuestionnaireId, Guid SectionId, AddQuestionnaireRequest Section) : IRequest<GetQuestionnaireResponse>;
public sealed record UpdateQuestionnaireCommand(Guid QuestionnaireId, UpdateQuestionnaireRequest Questionnaire) : IRequest<GetQuestionnaireResponse>;

#endregion

#region Validation

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

#endregion

#region Query Handlers

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

#endregion

#region Command Handlers

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
        else
        {
            await QuestionnaireGraph.EnsureNotReferenced(db, questionnaire.Id, cancellationToken);
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
        if (await QuestionnaireGraph.IsReferenced(db, q.Id, cancellationToken))
        {
            var version = await QuestionnaireGraph.CreateVersion(db, q, r.Questionnaire, cancellationToken);
            _ = await db.SaveAsync(nameof(UpdateQuestionnaireCommand), cancellationToken);
            await tx.CommitAsync(cancellationToken);
            return new() { Item = QuestionnaireGraph.Map(version) };
        }

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
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var questionnaire = await QuestionnaireGraph.Load(db, r.QuestionnaireId, true, cancellationToken);
        var sourceSection = questionnaire.Sections.SingleOrDefault(section => section.Id == r.SectionId && !section.IsDeleted) ?? throw new KeyNotFoundException("Questionnaire section was not found.");
        if (await QuestionnaireGraph.IsReferenced(db, questionnaire.Id, cancellationToken))
        {
            questionnaire = await QuestionnaireGraph.CreateVersion(db, questionnaire, QuestionnaireGraph.ToUpdateRequest(questionnaire), cancellationToken);
        }

        var section = questionnaire.Sections.Single(section => section.Code == sourceSection.Code);
        var count = await db.QuestionnaireQuestions.CountAsync(x => x.QuestionnaireSectionId == section.Id && !x.IsDeleted, cancellationToken);
        var order = Math.Min(r.Question.Order, count + 1);
        _ = await db.QuestionnaireQuestions.Where(x => x.QuestionnaireSectionId == section.Id && !x.IsDeleted && x.Order >= order)
            .ExecuteUpdateAsync(update => update.SetProperty(x => x.Order, x => x.Order + 1), cancellationToken);
        section.Questions.Add(new QuestionnaireQuestion { Code = r.Question.Code.Trim(), Label = r.Question.Name.Trim(), Hint = r.Question.Description?.Trim(), Type = r.Question.AnswerType, CompanyType = null, Order = order, IsRequired = r.Question.AnswerRule == QuestionnaireAnswerRule.Mandatory, IsVisible = true, IsActive = r.Question.IsActive, AnswerRule = r.Question.AnswerRule });
        _ = await db.SaveAsync(nameof(AddQuestionnaireQuestionCommand), cancellationToken);
        await tx.CommitAsync(cancellationToken);
        var q = await QuestionnaireGraph.Load(db, questionnaire.Id, false, cancellationToken);
        return new() { Item = QuestionnaireGraph.Map(q) };
    }
}
public sealed class UpdateQuestionnaireSectionHandler(IDatabaseService db) : IRequestHandler<UpdateQuestionnaireSectionCommand, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(UpdateQuestionnaireSectionCommand r, CancellationToken cancellationToken)
    {
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var questionnaire = await QuestionnaireGraph.Load(db, r.QuestionnaireId, true, cancellationToken);
        var sourceSection = questionnaire.Sections.SingleOrDefault(section => section.Id == r.SectionId && !section.IsDeleted) ?? throw new KeyNotFoundException("Questionnaire section was not found.");
        if (await QuestionnaireGraph.IsReferenced(db, questionnaire.Id, cancellationToken))
        {
            questionnaire = await QuestionnaireGraph.CreateVersion(db, questionnaire, QuestionnaireGraph.ToUpdateRequest(questionnaire), cancellationToken);
        }

        var section = questionnaire.Sections.Single(item => item.Code == sourceSection.Code);
        questionnaire.BusinessProcess = r.Section.BusinessProcess.Trim();
        section.Title = r.Section.Section.Trim();
        section.CompanyType = r.Section.VendorType;
        section.IsActive = r.Section.IsActive;
        _ = await db.SaveAsync(nameof(UpdateQuestionnaireSectionCommand), cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return new() { Item = QuestionnaireGraph.Map(await QuestionnaireGraph.Load(db, questionnaire.Id, false, cancellationToken)) };
    }
}

#endregion

#region Questionnaire Graph

internal static class QuestionnaireGraph
{
    public static async Task EnsureNotReferenced(IDatabaseService db, Guid questionnaireId, CancellationToken cancellationToken)
    {
        if (await IsReferenced(db, questionnaireId, cancellationToken))
        {
            throw new ValidationException("This questionnaire is already used by a vendor registration and cannot be changed. Create a new questionnaire version for future registrations.");
        }
    }

    public static Task<bool> IsReferenced(IDatabaseService db, Guid questionnaireId, CancellationToken cancellationToken)
    {
        return db.VendorRegistrations.AnyAsync(registration => registration.QuestionnaireId == questionnaireId, cancellationToken);
    }

    public static async Task<Questionnaire> CreateVersion(IDatabaseService db, Questionnaire source, UpdateQuestionnaireRequest request, CancellationToken cancellationToken)
    {
        byte[] expectedRowVersion;
        try
        {
            expectedRowVersion = Convert.FromBase64String(request.RowVersion);
        }
        catch (FormatException)
        {
            throw new ValidationException("The row version is invalid.");
        }

        if (!source.RowVersion.SequenceEqual(expectedRowVersion))
        {
            throw new ValidationException("The questionnaire has changed. Reload the page and try again.");
        }

        var codePrefix = source.Code.Split("_V", StringSplitOptions.None)[0];
        var versionNumber = 2;
        var code = $"{codePrefix}_V{versionNumber}";
        while (await db.Questionnaires.AnyAsync(questionnaire => questionnaire.Code == code, cancellationToken))
        {
            versionNumber++;
            code = $"{codePrefix}_V{versionNumber}";
        }

        var versionId = Guid.CreateVersion7();
        var version = new Questionnaire
        {
            Id = versionId,
            Code = code,
            BusinessProcess = request.BusinessProcess.Trim(),
            IsActive = request.IsActive
        };
        var questionIds = new Dictionary<Guid, Guid>();
        foreach (var sectionItem in request.Sections.OrderBy(item => item.Order))
        {
            var sectionId = Guid.CreateVersion7();
            var section = new QuestionnaireSection
            {
                Id = sectionId,
                QuestionnaireId = versionId,
                Code = sectionItem.Code,
                Title = sectionItem.Title,
                Order = sectionItem.Order,
                CompanyType = sectionItem.CompanyType,
                IsActive = sectionItem.IsActive
            };
            foreach (var questionItem in sectionItem.Questions.OrderBy(item => item.Order))
            {
                var questionId = Guid.CreateVersion7();
                questionIds[questionItem.Id] = questionId;
                var question = new QuestionnaireQuestion
                {
                    Id = questionId,
                    QuestionnaireSectionId = sectionId,
                    Code = questionItem.Code,
                    Label = questionItem.Label,
                    Hint = questionItem.Hint,
                    Placeholder = questionItem.Placeholder,
                    Type = questionItem.Type,
                    CompanyType = questionItem.CompanyType,
                    Order = questionItem.Order,
                    IsRequired = questionItem.AnswerRule == QuestionnaireAnswerRule.Mandatory,
                    IsVisible = questionItem.IsVisible,
                    IsActive = questionItem.IsActive,
                    AnswerRule = questionItem.AnswerRule
                };
                foreach (var optionItem in questionItem.Options.OrderBy(item => item.Order))
                {
                    question.Options.Add(new QuestionnaireOption
                    {
                        Id = Guid.CreateVersion7(),
                        QuestionnaireQuestionId = questionId,
                        Code = optionItem.Code,
                        Label = optionItem.Label,
                        Order = optionItem.Order
                    });
                }

                section.Questions.Add(question);
            }

            version.Sections.Add(section);
        }

        foreach (var ruleItem in request.Rules)
        {
            if (!questionIds.TryGetValue(ruleItem.SourceQuestionId, out var sourceQuestionId)
                || !questionIds.TryGetValue(ruleItem.TargetQuestionId, out var targetQuestionId))
            {
                throw new ValidationException("Rules must reference questions in the same questionnaire.");
            }

            version.Rules.Add(new QuestionnaireRule
            {
                Id = Guid.CreateVersion7(),
                QuestionnaireId = versionId,
                SourceQuestionId = sourceQuestionId,
                TargetQuestionId = targetQuestionId,
                Operator = ruleItem.Operator,
                Action = ruleItem.Action,
                Value = ruleItem.Value
            });
        }

        var errors = Validate(version, version.Rules.Select(rule => new QuestionnaireRuleItem(rule.Id, rule.SourceQuestionId, rule.TargetQuestionId, rule.Operator, rule.Action, rule.Value)).ToList());
        if (errors.Count != 0)
        {
            throw new ValidationException(string.Join("; ", errors));
        }

        if (version.IsActive)
        {
            source.IsActive = false;
        }

        _ = await db.Questionnaires.AddAsync(version, cancellationToken);
        return version;
    }

    public static UpdateQuestionnaireRequest ToUpdateRequest(Questionnaire questionnaire)
    {
        var item = Map(questionnaire);
        return new()
        {
            BusinessProcess = item.BusinessProcess,
            IsActive = item.IsActive,
            RowVersion = item.RowVersion,
            Sections = [.. item.Sections],
            Rules = [.. item.Rules]
        };
    }

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

#endregion
