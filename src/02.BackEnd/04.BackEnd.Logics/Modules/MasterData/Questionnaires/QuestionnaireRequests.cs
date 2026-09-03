using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires;

[AuthorizeRequestByPermission(QuestionnairePermissions.Read)] public sealed record GetQuestionnairesQuery : IRequest<GetQuestionnairesResponse>;
[AuthorizeRequestByPermission(QuestionnairePermissions.Read)] public sealed record GetQuestionnaireQuery(Guid Id) : IRequest<GetQuestionnaireResponse>;
[AuthorizeRequestByPermission(QuestionnairePermissions.Write)] public sealed record AddQuestionnaireCommand : AddQuestionnaireRequest, IRequest<GetQuestionnaireResponse>;
[AuthorizeRequestByPermission(QuestionnairePermissions.Write)] public sealed record UpdateQuestionnaireDraftCommand(Guid Id, UpdateQuestionnaireDraftRequest Draft) : IRequest<GetQuestionnaireResponse>;
[AuthorizeRequestByPermission(QuestionnairePermissions.Write)] public sealed record ValidateQuestionnaireCommand(Guid Id) : IRequest<ValidateQuestionnaireResponse>;
[AuthorizeRequestByPermission(QuestionnairePermissions.Publish)] public sealed record PublishQuestionnaireCommand(Guid Id) : IRequest;
[AuthorizeRequestByPermission(QuestionnairePermissions.Write)] public sealed record DeactivateQuestionnaireCommand(Guid Id) : IRequest;
[AuthorizeRequestByPermission(QuestionnairePermissions.Write)] public sealed record ArchiveQuestionnaireCommand(Guid Id) : IRequest;
[AuthorizeRequestByPermission(QuestionnairePermissions.Write)] public sealed record DeleteQuestionnaireDraftCommand(Guid Id) : IRequest;

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
        var rows = await db.QuestionnaireVersions.AsNoTracking().Where(v => !v.IsDeleted)
            .SelectMany(v => v.Sections.Where(s => !s.IsDeleted), (v, s) => new QuestionnaireListItem(v.QuestionnaireId, v.Id, v.Questionnaire.Code, v.Questionnaire.BusinessProcess, s.CompanyType, s.Title, v.Version, v.Status, v.IsActive))
            .OrderBy(x => x.BusinessProcess).ThenByDescending(x => x.Version).ThenBy(x => x.Section).ToListAsync(cancellationToken);
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
        if (await db.Questionnaires.AnyAsync(x => x.Code == r.Code, cancellationToken))
        {
            throw new InvalidOperationException($"Questionnaire code '{r.Code}' already exists.");
        }

        var q = new Questionnaire { Code = r.Code.Trim().ToUpperInvariant(), BusinessProcess = r.BusinessProcess.Trim() };
        var v = new QuestionnaireVersion { Questionnaire = q, Version = 1, Status = QuestionnaireVersionStatus.Draft, IsActive = false };
        q.Versions.Add(v);
        _ = await db.Questionnaires.AddAsync(q, cancellationToken);
        _ = await db.SaveAsync(nameof(AddQuestionnaireCommand), cancellationToken);
        return new() { Item = QuestionnaireGraph.Map(v) };
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
            var section = new QuestionnaireSection { QuestionnaireVersionId = v.Id, Code = s.Code, Title = s.Title, Order = s.Order, CompanyType = s.CompanyType };
            foreach (var q in s.Questions.OrderBy(x => x.Order))
            {
                var question = new QuestionnaireQuestion { Id = q.Id == Guid.Empty ? Guid.CreateVersion7() : q.Id, Code = q.Code, Label = q.Label, Hint = q.Hint, Placeholder = q.Placeholder, Type = q.Type, Order = q.Order, IsRequired = q.IsRequired, IsVisible = q.IsVisible };
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
            throw new InvalidOperationException("Delete drafts instead of archiving them.");
        }

        v.IsActive = false;
        v.Status = QuestionnaireVersionStatus.Archived;
        _ = await db.SaveAsync(nameof(ArchiveQuestionnaireCommand), cancellationToken);
    }
}
public sealed class DeleteQuestionnaireDraftHandler(IDatabaseService db) : IRequestHandler<DeleteQuestionnaireDraftCommand>
{
    public async Task Handle(DeleteQuestionnaireDraftCommand r, CancellationToken cancellationToken)
    {
        var v = await QuestionnaireGraph.Load(db, r.Id, false, cancellationToken);
        QuestionnaireGraph.EnsureDraft(v);
        _ = db.QuestionnaireVersions.Remove(v);
        _ = await db.SaveAsync(nameof(DeleteQuestionnaireDraftCommand), cancellationToken);
    }
}

internal static class QuestionnaireGraph
{
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
        return new(v.Id, v.QuestionnaireId, v.Questionnaire.Code, v.Questionnaire.BusinessProcess, v.Version, v.Status, v.IsActive, Convert.ToBase64String(v.RowVersion), v.Sections.OrderBy(x => x.Order).Select(s => new QuestionnaireSectionItem(s.Id, s.Code, s.Title, s.Order, s.CompanyType, s.Questions.OrderBy(x => x.Order).Select(q => new QuestionnaireQuestionItem(q.Id, q.Code, q.Label, q.Hint, q.Placeholder, q.Type, q.Order, q.IsRequired, q.IsVisible, q.Options.OrderBy(x => x.Order).Select(o => new QuestionnaireOptionItem(o.Id, o.Code, o.Label, o.Order)).ToList())).ToList())).ToList(), rules ?? []);
    }
}
