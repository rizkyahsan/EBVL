using System.Text.Json;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;
using Pertamina.Services.CurrentUser;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires;

// TODO: Restore questionnaire permissions after IdAMan permission setup is complete.
#region Requests

public sealed record GetQuestionnairesQuery : IRequest<GetQuestionnairesResponse>;
public sealed record GetQuestionnaireQuery(Guid QuestionnaireId) : IRequest<GetQuestionnaireResponse>;
public sealed record GetQuestionnaireVersionHistoryQuery(Guid QuestionnaireSeriesId) : IRequest<GetQuestionnaireVersionHistoryResponse>;
public sealed record GetQuestionnaireSectionsQuery(Guid QuestionnaireId) : GetQuestionnaireSectionsRequest, IRequest<GetQuestionnaireSectionsResponse>;
public sealed record GetQuestionnaireQuestionsQuery(Guid QuestionnaireId, Guid SectionId) : GetQuestionnaireQuestionsRequest, IRequest<GetQuestionnaireQuestionsResponse>;
public sealed record GetQuestionnaireQuestionQuery(Guid QuestionnaireId, Guid SectionId, Guid QuestionId) : IRequest<GetQuestionnaireQuestionResponse>;
public sealed record AddQuestionnaireCommand : AddQuestionnaireRequest, IRequest<GetQuestionnaireResponse>;
public sealed record CreateQuestionnaireDraftCommand(Guid QuestionnaireId) : IRequest<GetQuestionnaireResponse>;
public sealed record PublishQuestionnaireCommand(Guid QuestionnaireId, string RowVersion) : IRequest<GetQuestionnaireResponse>;
public sealed record AddQuestionnaireQuestionCommand(Guid QuestionnaireId, Guid SectionId, AddQuestionnaireQuestionRequest Question) : IRequest<GetQuestionnaireResponse>;
public sealed record AddQuestionnaireSectionCommand(Guid QuestionnaireId, AddQuestionnaireSectionRequest Section) : IRequest<GetQuestionnaireResponse>;
public sealed record UpdateQuestionnaireSectionCommand(Guid QuestionnaireId, Guid SectionId, UpdateQuestionnaireSectionRequest Section) : IRequest<GetQuestionnaireResponse>;
public sealed record DeleteQuestionnaireSectionCommand(Guid QuestionnaireId, Guid SectionId, string RowVersion) : IRequest<GetQuestionnaireResponse>;
public sealed record UpdateQuestionnaireQuestionCommand(Guid QuestionnaireId, Guid SectionId, Guid QuestionId, UpdateQuestionnaireQuestionRequest Question) : IRequest<GetQuestionnaireResponse>;
public sealed record DeleteQuestionnaireQuestionCommand(Guid QuestionnaireId, Guid SectionId, Guid QuestionId, string RowVersion) : IRequest<GetQuestionnaireResponse>;
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
public sealed class AddQuestionnaireSectionCommandValidator : AbstractValidatorBase<AddQuestionnaireSectionCommand>
{
    public AddQuestionnaireSectionCommandValidator()
    {
        _ = RuleFor(x => x.Section).SetValidator(new AddQuestionnaireSectionRequestValidator());
    }
}
public sealed class UpdateQuestionnaireSectionCommandValidator : AbstractValidatorBase<UpdateQuestionnaireSectionCommand>
{
    public UpdateQuestionnaireSectionCommandValidator()
    {
        _ = RuleFor(x => x.Section).SetValidator(new UpdateQuestionnaireSectionRequestValidator());
    }
}
public sealed class UpdateQuestionnaireQuestionCommandValidator : AbstractValidatorBase<UpdateQuestionnaireQuestionCommand>
{
    public UpdateQuestionnaireQuestionCommandValidator()
    {
        _ = RuleFor(x => x.Question).SetValidator(new UpdateQuestionnaireQuestionRequestValidator());
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
        var ids = await db.Questionnaires.AsNoTracking().Where(x => !x.IsDeleted)
            .Where(x => !db.Questionnaires.Any(other => !other.IsDeleted && other.QuestionnaireSeriesId == x.QuestionnaireSeriesId && other.Version > x.Version))
            .OrderBy(x => x.BusinessProcess).ThenBy(x => x.Code)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
        var rows = new List<QuestionnaireListItem>(ids.Count);
        foreach (var id in ids)
        {
            var q = await QuestionnaireGraph.Load(db, id, false, cancellationToken);
            var state = await QuestionnaireGraph.State(db, q, cancellationToken);
            rows.Add(new(q.Id, q.QuestionnaireSeriesId, q.Code, q.BusinessProcess, q.Version, q.Status, q.Modified ?? q.Created, q.Sections.Count, state.HasChanges, state.IsValidForPublish, state.CanPublish, state.Errors));
        }

        return new() { Items = rows };
    }
}
public sealed class GetQuestionnaireVersionHistoryHandler(IDatabaseService db) : IRequestHandler<GetQuestionnaireVersionHistoryQuery, GetQuestionnaireVersionHistoryResponse>
{
    public async Task<GetQuestionnaireVersionHistoryResponse> Handle(GetQuestionnaireVersionHistoryQuery request, CancellationToken cancellationToken)
    {
        var items = await db.Questionnaires.AsNoTracking()
            .Where(x => !x.IsDeleted && x.QuestionnaireSeriesId == request.QuestionnaireSeriesId)
            .OrderByDescending(x => x.Version)
            .Select(x => new QuestionnaireVersionItem(x.Id, x.Version, x.Status, x.PublishedAt, x.PublishedBy, x.Created))
            .ToListAsync(cancellationToken);
        return new() { Items = items };
    }
}
public sealed class GetQuestionnaireHandler(IDatabaseService db) : IRequestHandler<GetQuestionnaireQuery, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(GetQuestionnaireQuery r, CancellationToken cancellationToken)
    {
        var q = await QuestionnaireGraph.Load(db, r.QuestionnaireId, false, cancellationToken);
        return new() { Item = await QuestionnaireGraph.MapWithState(db, q, cancellationToken) };
    }
}

#endregion

#region Command Handlers

public sealed class AddQuestionnaireHandler(IDatabaseService db) : IRequestHandler<AddQuestionnaireCommand, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(AddQuestionnaireCommand r, CancellationToken cancellationToken)
    {
        var code = Code(r.Code);
        if (await db.Questionnaires.AnyAsync(x => x.Code == code && !x.IsDeleted, cancellationToken))
        {
            throw new ValidationException("A questionnaire business process with the same code already exists.");
        }

        var questionnaire = new Questionnaire { QuestionnaireSeriesId = Guid.CreateVersion7(), Code = code, BusinessProcess = r.BusinessProcess.Trim(), Version = 1, Status = QuestionnaireStatus.Draft, IsActive = r.IsActive };
        _ = await db.Questionnaires.AddAsync(questionnaire, cancellationToken);
        _ = await db.SaveAsync(nameof(AddQuestionnaireCommand), cancellationToken);
        return new() { Item = await QuestionnaireGraph.MapWithState(db, questionnaire, cancellationToken) };
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
public sealed class GetQuestionnaireSectionsHandler(IDatabaseService db) : IRequestHandler<GetQuestionnaireSectionsQuery, GetQuestionnaireSectionsResponse>
{
    public async Task<GetQuestionnaireSectionsResponse> Handle(GetQuestionnaireSectionsQuery r, CancellationToken cancellationToken)
    {
        _ = await db.Questionnaires.AsNoTracking().Where(x => x.Id == r.QuestionnaireId && !x.IsDeleted).Select(x => x.Id).SingleOrDefaultAsync(cancellationToken) is var id && id != Guid.Empty ? id : throw new KeyNotFoundException("Questionnaire was not found.");
        var query = db.QuestionnaireSections.AsNoTracking().Where(x => x.QuestionnaireId == r.QuestionnaireId && !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(r.SearchText))
        {
            query = query.Where(x => x.Code.Contains(r.SearchText) || x.Title.Contains(r.SearchText));
        }

        var desc = r.SortOrder == Pertamina.Common.Dto.Enums.SortOrder.Descending;
        var ordered = r.SortField?.ToLowerInvariant() switch
        {
            "code" => desc ? query.OrderByDescending(x => x.Code).ThenByDescending(x => x.Id) : query.OrderBy(x => x.Code).ThenBy(x => x.Id),
            "title" => desc ? query.OrderByDescending(x => x.Title).ThenByDescending(x => x.Id) : query.OrderBy(x => x.Title).ThenBy(x => x.Id),
            _ => desc ? query.OrderByDescending(x => x.Order).ThenByDescending(x => x.Id) : query.OrderBy(x => x.Order).ThenBy(x => x.Id)
        };
        var total = await query.CountAsync(cancellationToken);
        var page = r.Page ?? 1;
        var size = r.PageSize ?? 10;
        var items = await ordered.Skip((page - 1) * size).Take(size).Select(x => new QuestionnaireSectionListItem(x.Id, x.Code, x.Title, x.Questionnaire.BusinessProcess, x.CompanyType, x.Order, x.IsActive, x.Questions.Count(q => !q.IsDeleted))).ToListAsync(cancellationToken);
        return new() { Items = items, TotalCount = total };
    }
}
public sealed class GetQuestionnaireQuestionsHandler(IDatabaseService db) : IRequestHandler<GetQuestionnaireQuestionsQuery, GetQuestionnaireQuestionsResponse>
{
    public async Task<GetQuestionnaireQuestionsResponse> Handle(GetQuestionnaireQuestionsQuery r, CancellationToken cancellationToken)
    {
        var section = await QuestionnaireGraph.LoadSection(db, r.QuestionnaireId, r.SectionId, false, cancellationToken);
        var query = section.Questions.Where(x => !x.IsDeleted).AsQueryable();
        if (!string.IsNullOrWhiteSpace(r.SearchText))
        {
            query = query.Where(x => x.Code.Contains(r.SearchText, StringComparison.OrdinalIgnoreCase) || x.Label.Contains(r.SearchText, StringComparison.OrdinalIgnoreCase));
        }

        query = r.SortField?.ToLowerInvariant() switch { "code" => query.OrderBy(x => x.Code).ThenBy(x => x.Id), "label" => query.OrderBy(x => x.Label).ThenBy(x => x.Id), _ => query.OrderBy(x => x.Order).ThenBy(x => x.Id) };
        if (r.SortOrder == Pertamina.Common.Dto.Enums.SortOrder.Descending)
        {
            query = query.Reverse();
        }

        var total = query.Count();
        var page = r.Page ?? 1;
        var size = r.PageSize ?? 10;
        return new() { TotalCount = total, Items = query.Skip((page - 1) * size).Take(size).Select(QuestionnaireGraph.MapQuestion).ToList() };
    }
}
public sealed class GetQuestionnaireQuestionHandler(IDatabaseService db) : IRequestHandler<GetQuestionnaireQuestionQuery, GetQuestionnaireQuestionResponse>
{
    public async Task<GetQuestionnaireQuestionResponse> Handle(GetQuestionnaireQuestionQuery r, CancellationToken cancellationToken)
    {
        var section = await QuestionnaireGraph.LoadSection(db, r.QuestionnaireId, r.SectionId, false, cancellationToken);
        var question = section.Questions.SingleOrDefault(x => x.Id == r.QuestionId && !x.IsDeleted) ?? throw new KeyNotFoundException("Questionnaire question was not found.");
        return new() { Item = QuestionnaireGraph.MapQuestion(question) };
    }
}
public sealed class CreateQuestionnaireDraftHandler(IDatabaseService db) : IRequestHandler<CreateQuestionnaireDraftCommand, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(CreateQuestionnaireDraftCommand request, CancellationToken cancellationToken)
    {
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var source = await QuestionnaireGraph.Load(db, request.QuestionnaireId, true, cancellationToken);
        var existing = await db.Questionnaires.FirstOrDefaultAsync(x => !x.IsDeleted && x.QuestionnaireSeriesId == source.QuestionnaireSeriesId && x.Status == QuestionnaireStatus.Draft, cancellationToken);
        if (existing is not null)
        {
            await tx.CommitAsync(cancellationToken);
            return new() { Item = await QuestionnaireGraph.MapWithState(db, await QuestionnaireGraph.Load(db, existing.Id, false, cancellationToken), cancellationToken) };
        }

        if (source.Status != QuestionnaireStatus.Publish)
        {
            throw new ValidationException("A new draft can only be created from the published questionnaire.");
        }

        var draft = await QuestionnaireGraph.CreateVersion(db, source, QuestionnaireGraph.ToUpdateRequest(source), cancellationToken);
        _ = await db.SaveAsync(nameof(CreateQuestionnaireDraftCommand), cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return new() { Item = await QuestionnaireGraph.MapWithState(db, draft, cancellationToken) };
    }
}
public sealed class PublishQuestionnaireHandler(IDatabaseService db, ICurrentUserService currentUser) : IRequestHandler<PublishQuestionnaireCommand, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(PublishQuestionnaireCommand request, CancellationToken cancellationToken)
    {
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var draft = await QuestionnaireGraph.Load(db, request.QuestionnaireId, true, cancellationToken);
        QuestionnaireGraph.EnsureDraft(draft);
        await QuestionnaireGraph.EnsureNotReferenced(db, draft.Id, cancellationToken);
        db.SetQuestionnaireOriginalRowVersion(draft, QuestionnaireGraph.ParseRowVersion(request.RowVersion));
        var state = await QuestionnaireGraph.State(db, draft, cancellationToken);
        if (!state.HasChanges)
        {
            throw new ValidationException("The draft has no changes to publish.");
        }

        var errors = QuestionnaireGraph.Validate(draft, draft.Rules.Select(x => new QuestionnaireRuleItem(x.Id, x.SourceQuestionId, x.TargetQuestionId, x.Operator, x.Action, x.Value)).ToList());
        if (errors.Count != 0)
        {
            throw new ValidationException(string.Join("; ", errors));
        }

        var published = await db.Questionnaires.SingleOrDefaultAsync(x => !x.IsDeleted && x.QuestionnaireSeriesId == draft.QuestionnaireSeriesId && x.Status == QuestionnaireStatus.Publish, cancellationToken);
        try
        {
            if (published is not null)
            {
                published.Status = QuestionnaireStatus.Superseded;
                published.IsActive = false;
                _ = await db.SaveAsync(nameof(PublishQuestionnaireCommand), cancellationToken);
            }

            draft.Status = QuestionnaireStatus.Publish;
            draft.IsActive = true;
            draft.PublishedAt = DateTimeOffset.UtcNow;
            draft.PublishedBy = currentUser.Username ?? "EBVLSystem";
            _ = await db.SaveAsync(nameof(PublishQuestionnaireCommand), cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ValidationException("The questionnaire has changed or was already published. Reload the page and try again.");
        }
        catch (DbUpdateException)
        {
            throw new ValidationException("Another questionnaire version was published concurrently. Reload the page and try again.");
        }

        await tx.CommitAsync(cancellationToken);
        return new() { Item = await QuestionnaireGraph.MapWithState(db, draft, cancellationToken) };
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
public sealed class UpdateQuestionnaireSectionHandler(IDatabaseService db) : IRequestHandler<UpdateQuestionnaireSectionCommand, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(UpdateQuestionnaireSectionCommand r, CancellationToken cancellationToken)
    {
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var questionnaire = await QuestionnaireGraph.Load(db, r.QuestionnaireId, true, cancellationToken);
        QuestionnaireGraph.EnsureDraft(questionnaire);
        await QuestionnaireGraph.EnsureNotReferenced(db, questionnaire.Id, cancellationToken);
        db.SetQuestionnaireOriginalRowVersion(questionnaire, QuestionnaireGraph.ParseRowVersion(r.Section.RowVersion));
        questionnaire.Modified = DateTimeOffset.UtcNow;
        var section = questionnaire.Sections.SingleOrDefault(item => item.Id == r.SectionId && !item.IsDeleted) ?? throw new KeyNotFoundException("Questionnaire section was not found.");
        var code = r.Section.Code?.Trim() ?? section.Code;
        if (questionnaire.Sections.Any(item => item.Id != section.Id && !item.IsDeleted && item.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ValidationException("A section with the same code already exists in this questionnaire version.");
        }

        questionnaire.BusinessProcess = r.Section.BusinessProcess.Trim();
        section.Code = code;
        section.Title = r.Section.Section.Trim();
        section.CompanyType = r.Section.VendorType;
        section.IsActive = r.Section.IsActive;
        if (r.Section.Order is { } requestedOrder)
        {
            section.Order = Math.Clamp(requestedOrder, 1, questionnaire.Sections.Count);
            var nextOrder = 1;
            foreach (var item in questionnaire.Sections.Where(item => item.Id != section.Id && !item.IsDeleted).OrderBy(item => item.Order))
            {
                if (nextOrder == section.Order)
                {
                    nextOrder++;
                }

                item.Order = nextOrder++;
            }
        }

        await QuestionnaireGraph.SaveMutation(db, nameof(UpdateQuestionnaireSectionCommand), cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return new() { Item = await QuestionnaireGraph.MapWithState(db, await QuestionnaireGraph.Load(db, questionnaire.Id, false, cancellationToken), cancellationToken) };
    }
}
public sealed class AddQuestionnaireSectionHandler(IDatabaseService db) : IRequestHandler<AddQuestionnaireSectionCommand, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(AddQuestionnaireSectionCommand r, CancellationToken cancellationToken)
    {
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var q = await QuestionnaireGraph.Load(db, r.QuestionnaireId, true, cancellationToken);
        QuestionnaireGraph.EnsureDraft(q);
        await QuestionnaireGraph.EnsureNotReferenced(db, q.Id, cancellationToken);
        QuestionnaireGraph.EnsureRowVersion(q, r.Section.RowVersion);
        var code = r.Section.Code.Trim();
        if (q.Sections.Any(x => !x.IsDeleted && x.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ValidationException("A section with the same code already exists in this questionnaire version.");
        }

        var lastOrder = q.Sections.Where(x => !x.IsDeleted).Select(x => x.Order).DefaultIfEmpty().Max();
        var order = r.Section.Order <= 0 || r.Section.Order > lastOrder ? lastOrder + 1 : r.Section.Order;
        foreach (var item in q.Sections.Where(x => !x.IsDeleted && x.Order >= order))
        {
            item.Order++;
        }

        q.BusinessProcess = r.Section.BusinessProcess.Trim();
        q.Modified = DateTimeOffset.UtcNow;
        var section = new QuestionnaireSection { Id = Guid.CreateVersion7(), QuestionnaireId = q.Id, Code = code, Title = r.Section.Title.Trim(), CompanyType = r.Section.VendorType, Order = order, IsActive = r.Section.IsActive };
        _ = db.QuestionnaireSections.Add(section);
        await QuestionnaireGraph.SaveMutation(db, nameof(AddQuestionnaireSectionCommand), cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return new() { Item = await QuestionnaireGraph.MapWithState(db, q, cancellationToken) };
    }
}
public sealed class DeleteQuestionnaireSectionHandler(IDatabaseService db) : IRequestHandler<DeleteQuestionnaireSectionCommand, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(DeleteQuestionnaireSectionCommand r, CancellationToken cancellationToken)
    {
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var q = await QuestionnaireGraph.Load(db, r.QuestionnaireId, true, cancellationToken);
        QuestionnaireGraph.EnsureDraft(q);
        await QuestionnaireGraph.EnsureNotReferenced(db, q.Id, cancellationToken);
        db.SetQuestionnaireOriginalRowVersion(q, QuestionnaireGraph.ParseRowVersion(r.RowVersion));
        var section = q.Sections.SingleOrDefault(x => x.Id == r.SectionId && !x.IsDeleted) ?? throw new KeyNotFoundException("Questionnaire section was not found.");
        var questionIds = section.Questions.Select(x => x.Id).ToHashSet();
        if (q.Rules.Any(x => questionIds.Contains(x.SourceQuestionId) || questionIds.Contains(x.TargetQuestionId)))
        {
            throw new ValidationException("The section cannot be deleted while its questions are used by rules.");
        }

        _ = db.QuestionnaireSections.Remove(section);
        _ = q.Sections.Remove(section);
        var order = 1;
        foreach (var item in q.Sections.Where(x => !x.IsDeleted).OrderBy(x => x.Order))
        {
            item.Order = order++;
        }

        q.Modified = DateTimeOffset.UtcNow;
        await QuestionnaireGraph.SaveMutation(db, nameof(DeleteQuestionnaireSectionCommand), cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return new() { Item = await QuestionnaireGraph.MapWithState(db, q, cancellationToken) };
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
public sealed class DeleteQuestionnaireQuestionHandler(IDatabaseService db) : IRequestHandler<DeleteQuestionnaireQuestionCommand, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(DeleteQuestionnaireQuestionCommand r, CancellationToken cancellationToken)
    {
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var q = await QuestionnaireGraph.Load(db, r.QuestionnaireId, true, cancellationToken);
        QuestionnaireGraph.EnsureDraft(q);
        await QuestionnaireGraph.EnsureNotReferenced(db, q.Id, cancellationToken);
        db.SetQuestionnaireOriginalRowVersion(q, QuestionnaireGraph.ParseRowVersion(r.RowVersion));
        var section = q.Sections.SingleOrDefault(x => x.Id == r.SectionId && !x.IsDeleted) ?? throw new KeyNotFoundException("Questionnaire section was not found.");
        var question = section.Questions.SingleOrDefault(x => x.Id == r.QuestionId && !x.IsDeleted) ?? throw new KeyNotFoundException("Questionnaire question was not found.");
        if (q.Rules.Any(x => x.SourceQuestionId == question.Id || x.TargetQuestionId == question.Id))
        {
            throw new ValidationException("The question cannot be deleted while it is used by a rule.");
        }

        _ = db.QuestionnaireQuestions.Remove(question);
        _ = section.Questions.Remove(question);
        var order = 1;
        foreach (var item in section.Questions.Where(x => !x.IsDeleted).OrderBy(x => x.Order))
        {
            item.Order = order++;
        }

        q.Modified = DateTimeOffset.UtcNow;
        await QuestionnaireGraph.SaveMutation(db, nameof(DeleteQuestionnaireQuestionCommand), cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return new() { Item = await QuestionnaireGraph.MapWithState(db, q, cancellationToken) };
    }
}

#endregion

#region Questionnaire Graph

internal static class QuestionnaireGraph
{
    internal sealed record PublishState(bool HasChanges, bool IsValidForPublish, bool CanPublish, IReadOnlyList<string> Errors);

    public static void EnsureDraft(Questionnaire questionnaire)
    {
        if (questionnaire.Status != QuestionnaireStatus.Draft)
        {
            throw new ValidationException("Published and superseded questionnaire versions are read-only. Create or open the draft version to edit.");
        }
    }

    public static byte[] ParseRowVersion(string value)
    {
        try
        {
            return Convert.FromBase64String(value);
        }
        catch (FormatException)
        {
            throw new ValidationException("The row version is invalid.");
        }
    }
    public static void EnsureRowVersion(Questionnaire questionnaire, string value)
    {
        if (!questionnaire.RowVersion.SequenceEqual(ParseRowVersion(value)))
        {
            throw new ValidationException("The questionnaire has changed since this page was loaded. Reload the page and try again.");
        }
    }

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

        var versionNumber = await db.Questionnaires.Where(x => x.QuestionnaireSeriesId == source.QuestionnaireSeriesId).MaxAsync(x => x.Version, cancellationToken) + 1;

        var version = Clone(source, request, versionNumber);
        var errors = Validate(version, version.Rules.Select(rule => new QuestionnaireRuleItem(rule.Id, rule.SourceQuestionId, rule.TargetQuestionId, rule.Operator, rule.Action, rule.Value)).ToList(), false);
        if (errors.Count != 0)
        {
            throw new ValidationException(string.Join("; ", errors));
        }

        _ = await db.Questionnaires.AddAsync(version, cancellationToken);
        return version;
    }

    internal static Questionnaire Clone(Questionnaire source, UpdateQuestionnaireRequest request, int versionNumber)
    {
        var versionId = Guid.CreateVersion7();
        var version = new Questionnaire
        {
            Id = versionId,
            QuestionnaireSeriesId = source.QuestionnaireSeriesId,
            Code = source.Code,
            BusinessProcess = request.BusinessProcess.Trim(),
            Version = versionNumber,
            Status = QuestionnaireStatus.Draft,
            PreviousVersionId = source.Id,
            IsActive = false
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
    public static async Task<QuestionnaireSection> LoadSection(IDatabaseService db, Guid questionnaireId, Guid sectionId, bool tracking, CancellationToken ct)
    {
        var query = db.QuestionnaireSections.Include(x => x.Questionnaire).Include(x => x.Questions).ThenInclude(x => x.Options).Where(x => x.Id == sectionId && x.QuestionnaireId == questionnaireId && !x.IsDeleted && !x.Questionnaire.IsDeleted);
        if (!tracking)
        {
            query = query.AsNoTracking();
        }

        return await query.SingleOrDefaultAsync(ct) ?? throw new KeyNotFoundException("Questionnaire section was not found.");
    }
    public static QuestionnaireQuestionItem MapQuestion(QuestionnaireQuestion x)
    {
        return new(x.Id, x.Code, x.Label, x.Hint, x.Placeholder, x.Type, x.CompanyType, x.Order, x.IsRequired, x.IsVisible, x.IsActive, x.AnswerRule, x.Options.Where(o => !o.IsDeleted).OrderBy(o => o.Order).Select(o => new QuestionnaireOptionItem(o.Id, o.Code, o.Label, o.Order)).ToList());
    }

    public static QuestionnaireRuleItem MapRule(QuestionnaireRule x)
    {
        return new(x.Id, x.SourceQuestionId, x.TargetQuestionId, x.Operator, x.Action, x.Value);
    }

    public static async Task SaveMutation(IDatabaseService db, string action, CancellationToken ct)
    {
        try
        {
            _ = await db.SaveAsync(action, ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ValidationException("The questionnaire has changed since this page was loaded. Reload the page and try again.");
        }
        catch (DbUpdateException)
        {
            throw new ValidationException("The questionnaire change conflicts with existing data. Reload the page and try again.");
        }
    }
    public static async Task<PublishState> State(IDatabaseService db, Questionnaire questionnaire, CancellationToken ct)
    {
        Questionnaire? previous = null;
        if (questionnaire.PreviousVersionId is { } previousVersionId)
        {
            previous = await Load(db, previousVersionId, false, ct);
        }

        var hasChanges = questionnaire.Status == QuestionnaireStatus.Draft && HasEffectiveChanges(questionnaire, previous);
        var errors = Validate(questionnaire, questionnaire.Rules.Select(rule => new QuestionnaireRuleItem(rule.Id, rule.SourceQuestionId, rule.TargetQuestionId, rule.Operator, rule.Action, rule.Value)).ToList());
        var isValid = errors.Count == 0;
        var isReferenced = questionnaire.Status == QuestionnaireStatus.Draft && await IsReferenced(db, questionnaire.Id, ct);
        var publishErrors = errors.ToList();
        if (isReferenced)
        {
            publishErrors.Add("This draft is already used by a vendor registration and cannot be published.");
        }

        return new(hasChanges, isValid, questionnaire.Status == QuestionnaireStatus.Draft && hasChanges && isValid && !isReferenced, publishErrors);
    }
    public static async Task<QuestionnaireItem> MapWithState(IDatabaseService db, Questionnaire questionnaire, CancellationToken ct, IReadOnlyList<QuestionnaireRuleItem>? rules = null)
    {
        var state = await State(db, questionnaire, ct);
        return Map(questionnaire, rules, state);
    }
    internal static bool HasEffectiveChanges(Questionnaire draft, Questionnaire? previous)
    {
        if (previous is null)
        {
            return draft.Sections.Count != 0 || draft.Rules.Count != 0;
        }

        return Canonical(draft) != Canonical(previous);
    }
    private static string Canonical(Questionnaire questionnaire)
    {
        var questionKeys = questionnaire.Sections
            .SelectMany(section => section.Questions.Select(question => new { question.Id, Key = $"{Normalize(section.Code)}/{Normalize(question.Code)}" }))
            .ToDictionary(item => item.Id, item => item.Key);
        return JsonSerializer.Serialize(new
        {
            BusinessProcess = Normalize(questionnaire.BusinessProcess),
            Sections = questionnaire.Sections.OrderBy(section => section.Order).ThenBy(section => section.Code, StringComparer.OrdinalIgnoreCase).Select(section => new
            {
                Code = Normalize(section.Code),
                Title = Normalize(section.Title),
                section.Order,
                section.CompanyType,
                section.IsActive,
                Questions = section.Questions.OrderBy(question => question.Order).ThenBy(question => question.Code, StringComparer.OrdinalIgnoreCase).Select(question => new
                {
                    Code = Normalize(question.Code),
                    Label = Normalize(question.Label),
                    Hint = Normalize(question.Hint),
                    Placeholder = Normalize(question.Placeholder),
                    question.Type,
                    question.CompanyType,
                    question.Order,
                    question.IsVisible,
                    question.IsActive,
                    question.AnswerRule,
                    Options = question.Options.OrderBy(option => option.Order).ThenBy(option => option.Code, StringComparer.OrdinalIgnoreCase).Select(option => new
                    {
                        Code = Normalize(option.Code),
                        Label = Normalize(option.Label),
                        option.Order
                    })
                })
            }),
            Rules = questionnaire.Rules.Select(rule => new
            {
                Source = questionKeys.GetValueOrDefault(rule.SourceQuestionId, rule.SourceQuestionId.ToString()),
                Target = questionKeys.GetValueOrDefault(rule.TargetQuestionId, rule.TargetQuestionId.ToString()),
                rule.Operator,
                rule.Action,
                Value = Normalize(rule.Value)
            }).OrderBy(rule => rule.Source).ThenBy(rule => rule.Target).ThenBy(rule => rule.Operator).ThenBy(rule => rule.Action).ThenBy(rule => rule.Value)
        });
    }
    private static string Normalize(string? value)
    {
        return value?.Trim() ?? string.Empty;
    }
    public static List<string> Validate(Questionnaire q, IReadOnlyList<QuestionnaireRuleItem> rules, bool requirePublishable = true)
    {
        var errors = new List<string>();
        var questions = q.Sections.SelectMany(x => x.Questions).ToList();
        if (requirePublishable && q.Sections.Count == 0)
        {
            errors.Add("At least one section is required.");
        }

        if (requirePublishable && questions.Count == 0)
        {
            errors.Add("At least one question is required.");
        }

        if (requirePublishable && !q.Sections.Any(section => section.IsActive))
        {
            errors.Add("At least one active section is required.");
        }

        if (requirePublishable && !q.Sections.Any(section => section.IsActive && section.Questions.Any(question => question.IsActive)))
        {
            errors.Add("At least one active section must contain an active question.");
        }

        foreach (var g in q.Sections.GroupBy(x => x.Code, StringComparer.OrdinalIgnoreCase).Where(x => x.Count() > 1))
        {
            errors.Add($"Duplicate section code: {g.Key}.");
        }

        foreach (var order in q.Sections.GroupBy(section => section.Order).Where(group => group.Key < 1 || group.Count() > 1))
        {
            errors.Add($"Invalid or duplicate section order: {order.Key}.");
        }

        foreach (var g in questions.GroupBy(x => x.Code, StringComparer.OrdinalIgnoreCase).Where(x => x.Count() > 1))
        {
            errors.Add($"Duplicate question code: {g.Key}.");
        }

        foreach (var section in q.Sections)
        {
            foreach (var order in section.Questions.GroupBy(question => question.Order).Where(group => group.Key < 1 || group.Count() > 1))
            {
                errors.Add($"Invalid or duplicate question order in {section.Code}: {order.Key}.");
            }
        }

        foreach (var item in questions)
        {
            if (item.Order < 1)
            {
                errors.Add($"{item.Code} has an invalid order.");
            }

            var choice = item.Type is QuestionnaireQuestionType.SingleChoice or QuestionnaireQuestionType.MultipleChoice;
            if (choice && item.Options.Count == 0)
            {
                errors.Add($"{item.Code} requires options.");
            }

            if (!choice && item.Options.Count != 0)
            {
                errors.Add($"{item.Code} type does not support options.");
            }

            foreach (var duplicate in item.Options.GroupBy(x => x.Code, StringComparer.OrdinalIgnoreCase).Where(x => x.Count() > 1))
            {
                errors.Add($"Duplicate option code for {item.Code}: {duplicate.Key}.");
            }

            foreach (var order in item.Options.GroupBy(option => option.Order).Where(group => group.Key < 1 || group.Count() > 1))
            {
                errors.Add($"Invalid or duplicate option order for {item.Code}: {order.Key}.");
            }

            if (item.Options.Any(option => string.IsNullOrWhiteSpace(option.Code) || option.Code.Length > QuestionnaireMaximumLengthFor.Code || string.IsNullOrWhiteSpace(option.Label) || option.Label.Length > 300 || option.Order < 1))
            {
                errors.Add($"{item.Code} contains an invalid option.");
            }
        }

        var ids = questions.Select(x => x.Id).ToHashSet();
        foreach (var rule in rules)
        {
            if (!ids.Contains(rule.SourceQuestionId) || !ids.Contains(rule.TargetQuestionId))
            {
                errors.Add("Rules must reference questions in the same questionnaire.");
            }

            if (rule.SourceQuestionId == rule.TargetQuestionId)
            {
                errors.Add("A rule source and target question must be different.");
            }

            if (string.IsNullOrWhiteSpace(rule.Value) || rule.Value.Length > QuestionnaireMaximumLengthFor.RuleValue)
            {
                errors.Add("A rule contains an invalid comparison value.");
            }
        }

        return errors;
    }
    public static QuestionnaireItem Map(Questionnaire q, IReadOnlyList<QuestionnaireRuleItem>? rules = null, PublishState? state = null)
    {
        return new(q.Id, q.QuestionnaireSeriesId, q.Code, q.BusinessProcess, q.Version, q.Status, q.PreviousVersionId, q.PublishedAt, q.PublishedBy, q.IsActive, Convert.ToBase64String(q.RowVersion), q.Sections.OrderBy(x => x.Order).Select(s => new QuestionnaireSectionItem(s.Id, s.Code, s.Title, s.Order, s.CompanyType, s.IsActive, s.Questions.OrderBy(x => x.Order).Select(item => new QuestionnaireQuestionItem(item.Id, item.Code, item.Label, item.Hint, item.Placeholder, item.Type, item.CompanyType, item.Order, item.IsRequired, item.IsVisible, item.IsActive, item.AnswerRule, item.Options.OrderBy(x => x.Order).Select(o => new QuestionnaireOptionItem(o.Id, o.Code, o.Label, o.Order)).ToList())).ToList())).ToList(), rules ?? q.Rules.Select(x => new QuestionnaireRuleItem(x.Id, x.SourceQuestionId, x.TargetQuestionId, x.Operator, x.Action, x.Value)).ToList(), state?.HasChanges ?? false, state?.IsValidForPublish ?? false, state?.CanPublish ?? false, state?.Errors ?? []);
    }
}

#endregion
