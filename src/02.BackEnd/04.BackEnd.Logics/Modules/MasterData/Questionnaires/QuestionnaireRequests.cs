using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires;

// TODO: Restore QuestionnairePermissions.Read after IdAMan permission setup is complete.
public sealed record GetQuestionnairesQuery : IRequest<GetQuestionnairesResponse>;
public sealed record GetQuestionnaireQuery(Guid Id) : IRequest<GetQuestionnaireResponse>;
// TODO: Restore QuestionnairePermissions.Write and Publish after IdAMan permission setup is complete.
public sealed record AddQuestionnaireCommand : AddQuestionnaireRequest, IRequest<GetQuestionnaireResponse>;
public sealed record AddQuestionnaireQuestionCommand(Guid Id, Guid SectionId, AddQuestionnaireQuestionRequest Question) : IRequest<GetQuestionnaireResponse>;
public sealed record UpdateQuestionnaireDraftCommand(Guid Id, UpdateQuestionnaireDraftRequest Draft) : IRequest<GetQuestionnaireResponse>;
public sealed record ValidateQuestionnaireCommand(Guid Id) : IRequest<ValidateQuestionnaireResponse>;
public sealed record PublishQuestionnaireCommand(Guid Id) : IRequest;
public sealed record DeactivateQuestionnaireCommand(Guid Id) : IRequest;
public sealed record ArchiveQuestionnaireCommand(Guid Id) : IRequest;

public sealed class AddQuestionnaireCommandValidator : AbstractValidatorBase<AddQuestionnaireCommand>
{
    public AddQuestionnaireCommandValidator()
    {
        Include(new AddQuestionnaireRequestValidator());
    }
}
public sealed class UpdateQuestionnaireDraftCommandValidator : AbstractValidatorBase<UpdateQuestionnaireDraftCommand>
{
    public UpdateQuestionnaireDraftCommandValidator()
    {
        _ = RuleFor(x => x.Draft).SetValidator(new UpdateQuestionnaireDraftRequestValidator());
    }
}

public sealed class GetQuestionnairesHandler(IDatabaseService db) : IRequestHandler<GetQuestionnairesQuery, GetQuestionnairesResponse>
{
    public async Task<GetQuestionnairesResponse> Handle(GetQuestionnairesQuery r, CancellationToken cancellationToken)
    {
        var versions = await db.QuestionnaireVersions.AsNoTracking().Include(x => x.Questionnaire).Include(x => x.Sections).Where(v => !v.IsDeleted).ToListAsync(cancellationToken);
        var rows = versions.GroupBy(x => x.QuestionnaireId).Select(group => group.OrderByDescending(x => x.Status == QuestionnaireVersionStatus.Draft).ThenByDescending(x => x.IsActive).ThenByDescending(x => x.Version).First())
            .SelectMany(v => v.Sections.Where(s => !s.IsDeleted), (v, s) => new QuestionnaireListItem(v.QuestionnaireId, v.Id, s.Id, v.Questionnaire.Code, v.Questionnaire.BusinessProcess, s.CompanyType, s.Title, v.Version, v.Status, s.IsActive))
            .OrderBy(x => x.BusinessProcess).ThenBy(x => x.Section).ToList();
        return new() { Items = rows };
    }
}
public sealed class GetQuestionnaireHandler(IDatabaseService db) : IRequestHandler<GetQuestionnaireQuery, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(GetQuestionnaireQuery r, CancellationToken cancellationToken)
    {
        var version = await QuestionnaireGraph.Load(db, r.Id, false, cancellationToken);
        return new() { Item = QuestionnaireGraph.Map(version, await QuestionnaireGraph.Rules(db, version.Id, cancellationToken)) };
    }
}
public sealed class AddQuestionnaireHandler(IDatabaseService db) : IRequestHandler<AddQuestionnaireCommand, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(AddQuestionnaireCommand r, CancellationToken cancellationToken)
    {
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var code = Code(r.BusinessProcess);
        var questionnaire = await db.Questionnaires.Include(x => x.Versions).ThenInclude(x => x.Sections).ThenInclude(x => x.Questions).ThenInclude(x => x.Options)
            .Include(x => x.Versions).ThenInclude(x => x.Rules)
            .SingleOrDefaultAsync(x => x.Code == code, cancellationToken);
        QuestionnaireVersion version;
        if (questionnaire is null)
        {
            questionnaire = new Questionnaire { Code = code, BusinessProcess = r.BusinessProcess.Trim() };
            version = new QuestionnaireVersion { Questionnaire = questionnaire, Version = 1, Status = QuestionnaireVersionStatus.Draft, IsActive = false };
            questionnaire.Versions.Add(version);
            _ = await db.Questionnaires.AddAsync(questionnaire, cancellationToken);
        }
        else
        {
            version = questionnaire.Versions.SingleOrDefault(x => !x.IsDeleted && x.Status == QuestionnaireVersionStatus.Draft)
                ?? QuestionnaireGraph.CloneDraft(questionnaire);
        }

        var baseCode = Code(r.Section);
        var sectionCode = baseCode;
        for (var suffix = 2; version.Sections.Any(x => x.Code == sectionCode); suffix++)
        {
            sectionCode = $"{baseCode}_{suffix}";
        }

        version.Sections.Add(new QuestionnaireSection { Code = sectionCode, Title = r.Section.Trim(), Order = version.Sections.Count + 1, CompanyType = r.VendorType, IsActive = r.IsActive });
        _ = await db.SaveAsync(nameof(AddQuestionnaireCommand), cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return new() { Item = QuestionnaireGraph.Map(version) };
    }

    private static string Code(string value)
    {
        return string.Join('_', value.Trim().ToUpperInvariant().Split([' ', '-', '/'], StringSplitOptions.RemoveEmptyEntries));
    }
}
public sealed class UpdateQuestionnaireDraftHandler(IDatabaseService db) : IRequestHandler<UpdateQuestionnaireDraftCommand, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(UpdateQuestionnaireDraftCommand r, CancellationToken cancellationToken)
    {
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var v = await QuestionnaireGraph.Load(db, r.Id, true, cancellationToken);
        QuestionnaireGraph.EnsureDraft(v);
        db.SetQuestionnaireVersionOriginalRowVersion(v, Convert.FromBase64String(r.Draft.RowVersion));
        v.Questionnaire.BusinessProcess = r.Draft.BusinessProcess.Trim();
        db.QuestionnaireRules.RemoveRange(await db.QuestionnaireRules.Where(x => x.QuestionnaireVersionId == v.Id).ToListAsync(cancellationToken));
        db.QuestionnaireSections.RemoveRange(v.Sections);
        v.Sections.Clear();
        foreach (var s in r.Draft.Sections.OrderBy(x => x.Order))
        {
            var section = new QuestionnaireSection { QuestionnaireVersionId = v.Id, Code = s.Code, Title = s.Title, Order = s.Order, CompanyType = s.CompanyType, IsActive = s.IsActive };
            foreach (var q in s.Questions.OrderBy(x => x.Order))
            {
                var question = new QuestionnaireQuestion { Id = q.Id == Guid.Empty ? Guid.CreateVersion7() : q.Id, Code = q.Code, Label = q.Label, Hint = q.Hint, Placeholder = q.Placeholder, Type = q.Type, CompanyType = q.CompanyType, Order = q.Order, IsRequired = q.AnswerRule == QuestionnaireAnswerRule.Mandatory, IsVisible = q.IsVisible, IsActive = q.IsActive, AnswerRule = q.AnswerRule };
                foreach (var o in q.Options.OrderBy(x => x.Order))
                {
                    question.Options.Add(new QuestionnaireOption { Code = o.Code, Label = o.Label, Order = o.Order });
                }

                section.Questions.Add(question);
            }

            v.Sections.Add(section);
        }

        foreach (var rule in r.Draft.Rules)
        {
            _ = await db.QuestionnaireRules.AddAsync(new QuestionnaireRule { QuestionnaireVersionId = v.Id, SourceQuestionId = rule.SourceQuestionId, TargetQuestionId = rule.TargetQuestionId, Operator = rule.Operator, Action = rule.Action, Value = rule.Value }, cancellationToken);
        }

        var errors = QuestionnaireGraph.Validate(v, r.Draft.Rules);
        if (errors.Count != 0)
        {
            throw new ValidationException(string.Join("; ", errors));
        }

        _ = await db.SaveAsync(nameof(UpdateQuestionnaireDraftCommand), cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return new() { Item = QuestionnaireGraph.Map(v, r.Draft.Rules) };
    }
}
public sealed class AddQuestionnaireQuestionHandler(IDatabaseService db) : IRequestHandler<AddQuestionnaireQuestionCommand, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(AddQuestionnaireQuestionCommand r, CancellationToken cancellationToken)
    {
        var section = await db.QuestionnaireSections.AsNoTracking()
            .Where(x => x.Id == r.SectionId && x.QuestionnaireVersionId == r.Id && !x.IsDeleted)
            .Select(x => new { x.Id, x.QuestionnaireVersion.Status })
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException("Questionnaire section was not found.");
        if (section.Status != QuestionnaireVersionStatus.Draft)
        {
            throw new InvalidOperationException("Published questionnaire versions are immutable.");
        }

        var order = await db.QuestionnaireQuestions.AsNoTracking().CountAsync(x => x.QuestionnaireSectionId == section.Id && !x.IsDeleted, cancellationToken) + 1;
        _ = await db.QuestionnaireQuestions.AddAsync(new QuestionnaireQuestion
        {
            QuestionnaireSectionId = section.Id,
            Code = r.Question.Code.Trim(),
            Label = r.Question.Name.Trim(),
            Hint = r.Question.Description?.Trim(),
            Type = r.Question.AnswerType,
            CompanyType = r.Question.VendorType,
            Order = order,
            IsRequired = r.Question.AnswerRule == QuestionnaireAnswerRule.Mandatory,
            IsVisible = true,
            IsActive = r.Question.IsActive,
            AnswerRule = r.Question.AnswerRule
        }, cancellationToken);
        _ = await db.SaveAsync(nameof(AddQuestionnaireQuestionCommand), cancellationToken);
        var version = await QuestionnaireGraph.Load(db, r.Id, false, cancellationToken);
        return new() { Item = QuestionnaireGraph.Map(version, await QuestionnaireGraph.Rules(db, version.Id, cancellationToken)) };
    }
}
public sealed class ValidateQuestionnaireHandler(IDatabaseService db) : IRequestHandler<ValidateQuestionnaireCommand, ValidateQuestionnaireResponse>
{
    public async Task<ValidateQuestionnaireResponse> Handle(ValidateQuestionnaireCommand r, CancellationToken cancellationToken)
    {
        var v = await QuestionnaireGraph.Load(db, r.Id, false, cancellationToken);
        var rules = await db.QuestionnaireRules.AsNoTracking().Where(x => x.QuestionnaireVersionId == v.Id).Select(x => new QuestionnaireRuleItem(x.Id, x.SourceQuestionId, x.TargetQuestionId, x.Operator, x.Action, x.Value)).ToListAsync(cancellationToken);
        return new() { Errors = QuestionnaireGraph.Validate(v, rules) };
    }
}
public sealed class PublishQuestionnaireHandler(IDatabaseService db) : IRequestHandler<PublishQuestionnaireCommand>
{
    public async Task Handle(PublishQuestionnaireCommand r, CancellationToken cancellationToken)
    {
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var v = await QuestionnaireGraph.Load(db, r.Id, true, cancellationToken);
        QuestionnaireGraph.EnsureDraft(v);
        var errors = QuestionnaireGraph.Validate(v, await QuestionnaireGraph.Rules(db, v.Id, cancellationToken));
        if (errors.Count != 0)
        {
            throw new ValidationException(string.Join("; ", errors));
        }

        foreach (var old in await db.QuestionnaireVersions.Where(x => x.QuestionnaireId == v.QuestionnaireId && x.IsActive).ToListAsync(cancellationToken))
        {
            old.IsActive = false;
        }

        v.Status = QuestionnaireVersionStatus.Published;
        v.IsActive = true;
        v.PublishedAt = DateTimeOffset.UtcNow;
        v.Questionnaire.PublishedVersionId = v.Id;
        _ = await db.SaveAsync(nameof(PublishQuestionnaireCommand), cancellationToken);
        await tx.CommitAsync(cancellationToken);
    }
}
public sealed class DeactivateQuestionnaireHandler(IDatabaseService db) : IRequestHandler<DeactivateQuestionnaireCommand>
{
    public async Task Handle(DeactivateQuestionnaireCommand r, CancellationToken cancellationToken)
    {
        var v = await QuestionnaireGraph.Load(db, r.Id, false, cancellationToken);
        if (v.Status != QuestionnaireVersionStatus.Published)
        {
            throw new InvalidOperationException("Only published versions can be deactivated.");
        }

        v.IsActive = false;
        v.Status = QuestionnaireVersionStatus.Inactive;
        _ = await db.SaveAsync(nameof(DeactivateQuestionnaireCommand), cancellationToken);
    }
}
public sealed class ArchiveQuestionnaireHandler(IDatabaseService db) : IRequestHandler<ArchiveQuestionnaireCommand>
{
    public async Task Handle(ArchiveQuestionnaireCommand r, CancellationToken cancellationToken)
    {
        var v = await QuestionnaireGraph.Load(db, r.Id, false, cancellationToken);
        if (v.Status == QuestionnaireVersionStatus.Draft)
        {
            throw new InvalidOperationException("Draft questionnaires cannot be archived.");
        }

        v.IsActive = false;
        v.Status = QuestionnaireVersionStatus.Archived;
        _ = await db.SaveAsync(nameof(ArchiveQuestionnaireCommand), cancellationToken);
    }
}
internal static class QuestionnaireGraph
{
    public static QuestionnaireVersion CloneDraft(Questionnaire questionnaire)
    {
        var source = questionnaire.Versions.OrderByDescending(x => x.Version).First();
        var draft = new QuestionnaireVersion { Questionnaire = questionnaire, Version = source.Version + 1, Status = QuestionnaireVersionStatus.Draft, IsActive = false };
        var questionIds = new Dictionary<Guid, Guid>();
        foreach (var sourceSection in source.Sections.Where(x => !x.IsDeleted).OrderBy(x => x.Order))
        {
            var section = new QuestionnaireSection { Code = sourceSection.Code, Title = sourceSection.Title, Order = sourceSection.Order, CompanyType = sourceSection.CompanyType, IsActive = sourceSection.IsActive };
            foreach (var sourceQuestion in sourceSection.Questions.Where(x => !x.IsDeleted).OrderBy(x => x.Order))
            {
                var question = new QuestionnaireQuestion { Code = sourceQuestion.Code, Label = sourceQuestion.Label, Hint = sourceQuestion.Hint, Placeholder = sourceQuestion.Placeholder, Type = sourceQuestion.Type, CompanyType = sourceQuestion.CompanyType, Order = sourceQuestion.Order, IsRequired = sourceQuestion.IsRequired, IsVisible = sourceQuestion.IsVisible, IsActive = sourceQuestion.IsActive, AnswerRule = sourceQuestion.AnswerRule };
                questionIds[sourceQuestion.Id] = question.Id;
                foreach (var option in sourceQuestion.Options.Where(x => !x.IsDeleted).OrderBy(x => x.Order))
                {
                    question.Options.Add(new QuestionnaireOption { Code = option.Code, Label = option.Label, Order = option.Order });
                }

                section.Questions.Add(question);
            }

            draft.Sections.Add(section);
        }

        foreach (var rule in source.Rules.Where(x => !x.IsDeleted))
        {
            draft.Rules.Add(new QuestionnaireRule { SourceQuestionId = questionIds[rule.SourceQuestionId], TargetQuestionId = questionIds[rule.TargetQuestionId], Operator = rule.Operator, Action = rule.Action, Value = rule.Value });
        }

        questionnaire.Versions.Add(draft);
        return draft;
    }

    public static async Task<QuestionnaireVersion> Load(IDatabaseService db, Guid id, bool tracking, CancellationToken ct)
    {
        var query = db.QuestionnaireVersions.Include(x => x.Questionnaire).Include(x => x.Sections).ThenInclude(x => x.Questions).ThenInclude(x => x.Options).Where(x => x.Id == id && !x.IsDeleted);
        if (!tracking)
        {
            query = query.AsNoTracking();
        }

        return await query.SingleOrDefaultAsync(ct) ?? throw new InvalidOperationException("Questionnaire version was not found.");
    }
    public static void EnsureDraft(QuestionnaireVersion v)
    {
        if (v.Status != QuestionnaireVersionStatus.Draft)
        {
            throw new InvalidOperationException("Published questionnaire versions are immutable.");
        }
    }
    public static Task<List<QuestionnaireRuleItem>> Rules(IDatabaseService db, Guid id, CancellationToken ct)
    {
        return db.QuestionnaireRules.AsNoTracking().Where(x => x.QuestionnaireVersionId == id).Select(x => new QuestionnaireRuleItem(x.Id, x.SourceQuestionId, x.TargetQuestionId, x.Operator, x.Action, x.Value)).ToListAsync(ct);
    }

    public static List<string> Validate(QuestionnaireVersion v, IReadOnlyList<QuestionnaireRuleItem> rules)
    {
        var errors = new List<string>();
        var questions = v.Sections.SelectMany(x => x.Questions).ToList();
        if (v.Sections.Count == 0)
        {
            errors.Add("At least one section is required.");
        }

        if (questions.Count == 0)
        {
            errors.Add("At least one question is required.");
        }

        foreach (var g in v.Sections.GroupBy(x => x.Code, StringComparer.OrdinalIgnoreCase).Where(x => x.Count() > 1))
        {
            errors.Add($"Duplicate section code: {g.Key}.");
        }

        foreach (var g in questions.GroupBy(x => x.Code, StringComparer.OrdinalIgnoreCase).Where(x => x.Count() > 1))
        {
            errors.Add($"Duplicate question code: {g.Key}.");
        }

        foreach (var q in questions)
        {
            var choice = q.Type is QuestionnaireQuestionType.SingleChoice or QuestionnaireQuestionType.MultipleChoice;
            if (choice && q.Options.Count == 0)
            {
                errors.Add($"{q.Code} requires options.");
            }

            if (!choice && q.Options.Count != 0)
            {
                errors.Add($"{q.Code} type does not support options.");
            }

            if (q.Type == QuestionnaireQuestionType.File && q.Options.Count != 0)
            {
                errors.Add($"{q.Code} file questions cannot have options.");
            }
        }

        var ids = questions.Select(x => x.Id).ToHashSet();
        foreach (var r in rules)
        {
            if (!ids.Contains(r.SourceQuestionId) || !ids.Contains(r.TargetQuestionId))
            {
                errors.Add("Rules must reference questions in the same version.");
            }
        }

        var edges = rules.GroupBy(x => x.SourceQuestionId).ToDictionary(x => x.Key, x => x.Select(y => y.TargetQuestionId));
        var visiting = new HashSet<Guid>();
        var done = new HashSet<Guid>();
        bool Cycle(Guid n)
        {
            if (done.Contains(n))
            {
                return false;
            }

            if (!visiting.Add(n))
            {
                return true;
            }

            if (edges.TryGetValue(n, out var next) && next.Any(Cycle))
            {
                return true;
            }

            _ = visiting.Remove(n);
            _ = done.Add(n);
            return false;
        }

        if (ids.Any(Cycle))
        {
            errors.Add("Questionnaire rules contain a cycle.");
        }

        return errors;
    }
    public static QuestionnaireDraftItem Map(QuestionnaireVersion v, IReadOnlyList<QuestionnaireRuleItem>? rules = null)
    {
        return new(v.Id, v.QuestionnaireId, v.Questionnaire.Code, v.Questionnaire.BusinessProcess, v.Version, v.Status, v.IsActive, Convert.ToBase64String(v.RowVersion), v.Sections.OrderBy(x => x.Order).Select(s => new QuestionnaireSectionItem(s.Id, s.Code, s.Title, s.Order, s.CompanyType, s.IsActive, s.Questions.OrderBy(x => x.Order).Select(q => new QuestionnaireQuestionItem(q.Id, q.Code, q.Label, q.Hint, q.Placeholder, q.Type, q.CompanyType, q.Order, q.IsRequired, q.IsVisible, q.IsActive, q.AnswerRule, q.Options.OrderBy(x => x.Order).Select(o => new QuestionnaireOptionItem(o.Id, o.Code, o.Label, o.Order)).ToList())).ToList())).ToList(), rules ?? []);
    }
}
