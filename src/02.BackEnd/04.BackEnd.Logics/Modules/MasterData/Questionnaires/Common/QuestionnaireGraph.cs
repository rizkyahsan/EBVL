using System.Text.Json;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.Common;

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
