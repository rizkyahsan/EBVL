using EBVL.BackEnd.Domain.Entities;
using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;
using EBVL.Shared.Enums;
using FluentValidation;
using Xunit;

namespace EBVL.BackEnd.Logics.Tests;

public sealed class QuestionnaireLifecycleTests
{
    [Fact]
    public void QuestionnaireStatusContractIsStable()
    {
        Assert.Equal(0, (int)QuestionnaireStatus.Draft);
        Assert.Equal(1, (int)QuestionnaireStatus.Publish);
        Assert.Equal(2, (int)QuestionnaireStatus.Superseded);
        Assert.Equal(["Draft", "Publish", "Superseded"], Enum.GetNames<QuestionnaireStatus>());
    }

    [Fact]
    public void CloneCreatesNextDraftInSameSeries()
    {
        var (source, request) = CreateSource();

        var draft = QuestionnaireGraph.Clone(source, request, 2);

        Assert.Equal(source.QuestionnaireSeriesId, draft.QuestionnaireSeriesId);
        Assert.Equal(2, draft.Version);
        Assert.Equal(QuestionnaireStatus.Draft, draft.Status);
        Assert.Equal(source.Id, draft.PreviousVersionId);
        Assert.False(draft.IsActive);
    }

    [Fact]
    public void CloneAssignsNewAggregateAndChildIds()
    {
        var (source, request) = CreateSource();

        var draft = QuestionnaireGraph.Clone(source, request, 2);

        Assert.NotEqual(source.Id, draft.Id);
        Assert.NotEqual(source.Sections.Single().Id, draft.Sections.Single().Id);
        Assert.DoesNotContain(draft.Sections.SelectMany(x => x.Questions), question => source.Sections.SelectMany(x => x.Questions).Any(old => old.Id == question.Id));
        Assert.DoesNotContain(draft.Sections.SelectMany(x => x.Questions).SelectMany(x => x.Options), option => source.Sections.SelectMany(x => x.Questions).SelectMany(x => x.Options).Any(old => old.Id == option.Id));
    }

    [Fact]
    public void CloneRemapsRuleToClonedQuestions()
    {
        var (source, request) = CreateSource();

        var draft = QuestionnaireGraph.Clone(source, request, 2);
        var questionIds = draft.Sections.SelectMany(x => x.Questions).Select(x => x.Id).ToHashSet();
        var rule = Assert.Single(draft.Rules);

        Assert.Contains(rule.SourceQuestionId, questionIds);
        Assert.Contains(rule.TargetQuestionId, questionIds);
        Assert.DoesNotContain(rule.SourceQuestionId, source.Sections.SelectMany(x => x.Questions).Select(x => x.Id));
    }

    [Fact]
    public void CloneDoesNotModifyPublishedSource()
    {
        var (source, request) = CreateSource();

        var draft = QuestionnaireGraph.Clone(source, request, 2);
        draft.Sections.Single().Questions.First().Label = "Changed";

        Assert.Equal("Source", source.Sections.Single().Questions.First().Label);
        Assert.Equal(QuestionnaireStatus.Publish, source.Status);
    }

    [Fact]
    public void CloneRejectsRuleOutsideAggregate()
    {
        var (source, request) = CreateSource();
        request.Rules[0] = request.Rules[0] with { SourceQuestionId = Guid.NewGuid() };

        var exception = Assert.Throws<ValidationException>(() => QuestionnaireGraph.Clone(source, request, 2));

        Assert.Contains("same questionnaire", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(QuestionnaireStatus.Publish)]
    [InlineData(QuestionnaireStatus.Superseded)]
    public void HistoricalVersionsAreReadOnly(QuestionnaireStatus status)
    {
        var (questionnaire, _) = CreateSource();
        questionnaire.Status = status;

        _ = Assert.Throws<ValidationException>(() => QuestionnaireGraph.EnsureDraft(questionnaire));
    }

    [Fact]
    public void DraftCanBeEdited()
    {
        var (questionnaire, _) = CreateSource();
        questionnaire.Status = QuestionnaireStatus.Draft;

        var exception = Record.Exception(() => QuestionnaireGraph.EnsureDraft(questionnaire));

        Assert.Null(exception);
    }

    [Fact]
    public void ValidationRejectsQuestionnaireWithoutSections()
    {
        var questionnaire = new Questionnaire { QuestionnaireSeriesId = Guid.NewGuid(), Code = "EMPTY", BusinessProcess = "Vendor Registration" };

        var errors = QuestionnaireGraph.Validate(questionnaire, []);

        Assert.Contains(errors, error => error.Contains("section", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(errors, error => error.Contains("question", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidationRejectsChoiceWithoutOptions()
    {
        var (source, _) = CreateSource();
        source.Sections.Single().Questions.First().Type = QuestionnaireQuestionType.SingleChoice;
        source.Sections.Single().Questions.First().Options.Clear();

        var errors = QuestionnaireGraph.Validate(source, []);

        Assert.Contains(errors, error => error.Contains("requires options", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void HistoryMappingPreservesConcreteVersionMetadata()
    {
        var (source, _) = CreateSource();

        var item = QuestionnaireGraph.Map(source);

        Assert.Equal(source.Id, item.QuestionnaireId);
        Assert.Equal(source.QuestionnaireSeriesId, item.QuestionnaireSeriesId);
        Assert.Equal(1, item.Version);
        Assert.Equal(QuestionnaireStatus.Publish, item.Status);
    }

    [Fact]
    public void IdenticalCloneHasNoEffectiveChanges()
    {
        var (source, request) = CreateSource();
        var draft = QuestionnaireGraph.Clone(source, request, 2);

        Assert.False(QuestionnaireGraph.HasEffectiveChanges(draft, source));
    }

    [Fact]
    public void AuditMetadataDoesNotCreateEffectiveChanges()
    {
        var (source, request) = CreateSource();
        var draft = QuestionnaireGraph.Clone(source, request, 2);
        draft.Modified = DateTimeOffset.UtcNow;
        draft.ModifiedBy = "another-user";

        Assert.False(QuestionnaireGraph.HasEffectiveChanges(draft, source));
    }

    [Theory]
    [InlineData("Section")]
    [InlineData("Question")]
    [InlineData("Option")]
    [InlineData("Rule")]
    public void BusinessGraphChangeCreatesEffectiveChanges(string target)
    {
        var (source, request) = CreateSource();
        var draft = QuestionnaireGraph.Clone(source, request, 2);
        switch (target)
        {
            case "Section":
                draft.Sections.Single().Title = "Changed section";
                break;
            case "Question":
                draft.Sections.Single().Questions.First().Label = "Changed question";
                break;
            case "Option":
                draft.Sections.Single().Questions.First().Options.Single().Label = "Changed option";
                break;
            case "Rule":
                draft.Rules.Single().Value = "NO";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(target));
        }

        Assert.True(QuestionnaireGraph.HasEffectiveChanges(draft, source));
    }

    [Fact]
    public void RevertedBusinessChangeHasNoEffectiveChanges()
    {
        var (source, request) = CreateSource();
        var draft = QuestionnaireGraph.Clone(source, request, 2);
        var question = draft.Sections.Single().Questions.First();
        var original = question.Label;
        question.Label = "Temporary";
        question.Label = original;

        Assert.False(QuestionnaireGraph.HasEffectiveChanges(draft, source));
    }

    [Fact]
    public void NewVersionOneDraftIsChangedOnlyAfterContentExists()
    {
        var empty = new Questionnaire { QuestionnaireSeriesId = Guid.NewGuid(), Code = "NEW", BusinessProcess = "Vendor Registration", Status = QuestionnaireStatus.Draft };

        Assert.False(QuestionnaireGraph.HasEffectiveChanges(empty, null));
        empty.Sections.Add(new QuestionnaireSection { QuestionnaireId = empty.Id, Code = "GENERAL", Title = "General", Order = 1 });
        Assert.True(QuestionnaireGraph.HasEffectiveChanges(empty, null));
    }

    [Fact]
    public void InvalidDraftIsNotValidForPublish()
    {
        var questionnaire = new Questionnaire { QuestionnaireSeriesId = Guid.NewGuid(), Code = "INVALID", BusinessProcess = "Vendor Registration", Status = QuestionnaireStatus.Draft };

        var errors = QuestionnaireGraph.Validate(questionnaire, []);

        Assert.NotEmpty(errors);
    }

    [Fact]
    public void ValidChangedDraftPassesPublishValidation()
    {
        var (source, request) = CreateSource();
        var draft = QuestionnaireGraph.Clone(source, request, 2);
        draft.Sections.Single().Questions.Last().Label = "Changed";

        var errors = QuestionnaireGraph.Validate(draft, draft.Rules.Select(rule => new QuestionnaireRuleItem(rule.Id, rule.SourceQuestionId, rule.TargetQuestionId, rule.Operator, rule.Action, rule.Value)).ToList());

        Assert.True(QuestionnaireGraph.HasEffectiveChanges(draft, source));
        Assert.Empty(errors);
    }

    [Fact]
    public void ValidationRejectsDuplicateSectionCode()
    {
        var (source, _) = CreateSource();
        source.Sections.Add(new QuestionnaireSection { QuestionnaireId = source.Id, Code = "general", Title = "Duplicate", Order = 2 });

        var errors = QuestionnaireGraph.Validate(source, []);

        Assert.Contains(errors, error => error.Contains("Duplicate section code", StringComparison.Ordinal));
    }

    [Fact]
    public void ValidationRejectsDuplicateQuestionOrder()
    {
        var (source, _) = CreateSource();
        source.Sections.Single().Questions.Last().Order = 1;

        var errors = QuestionnaireGraph.Validate(source, []);

        Assert.Contains(errors, error => error.Contains("duplicate question order", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidationRejectsDuplicateOptionOrder()
    {
        var (source, _) = CreateSource();
        var question = source.Sections.Single().Questions.First();
        question.Options.Add(new QuestionnaireOption { QuestionnaireQuestionId = question.Id, Code = "NO", Label = "No", Order = 1 });

        var errors = QuestionnaireGraph.Validate(source, []);

        Assert.Contains(errors, error => error.Contains("duplicate option order", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void DraftMutationAllowsSectionBeforeQuestionIsAdded()
    {
        var questionnaire = new Questionnaire { QuestionnaireSeriesId = Guid.NewGuid(), Code = "NEW", BusinessProcess = "Vendor Registration", Status = QuestionnaireStatus.Draft };
        questionnaire.Sections.Add(new QuestionnaireSection { QuestionnaireId = questionnaire.Id, Code = "VENDOR_REGISTRATION", Title = "New Section", Order = 1 });

        var saveErrors = QuestionnaireGraph.Validate(questionnaire, [], false);
        var publishErrors = QuestionnaireGraph.Validate(questionnaire, []);

        Assert.Empty(saveErrors);
        Assert.Contains(publishErrors, error => error.Contains("question", StringComparison.OrdinalIgnoreCase));
    }

    private static (Questionnaire Source, UpdateQuestionnaireRequest Request) CreateSource()
    {
        var sourceId = Guid.NewGuid();
        var sectionId = Guid.NewGuid();
        var sourceQuestionId = Guid.NewGuid();
        var targetQuestionId = Guid.NewGuid();
        var optionId = Guid.NewGuid();
        var source = new Questionnaire
        {
            Id = sourceId,
            QuestionnaireSeriesId = Guid.NewGuid(),
            Code = "VENDOR_REGISTRATION",
            BusinessProcess = "Vendor Registration",
            Version = 1,
            Status = QuestionnaireStatus.Publish,
            IsActive = true
        };
        var section = new QuestionnaireSection { Id = sectionId, QuestionnaireId = sourceId, Code = "GENERAL", Title = "General", Order = 1 };
        var first = new QuestionnaireQuestion { Id = sourceQuestionId, QuestionnaireSectionId = sectionId, Code = "SOURCE", Label = "Source", Type = QuestionnaireQuestionType.SingleChoice, Order = 1, IsVisible = true, IsActive = true, AnswerRule = QuestionnaireAnswerRule.Mandatory };
        first.Options.Add(new QuestionnaireOption { Id = optionId, QuestionnaireQuestionId = sourceQuestionId, Code = "YES", Label = "Yes", Order = 1 });
        var second = new QuestionnaireQuestion { Id = targetQuestionId, QuestionnaireSectionId = sectionId, Code = "TARGET", Label = "Target", Type = QuestionnaireQuestionType.ShortText, Order = 2, IsVisible = true, IsActive = true, AnswerRule = QuestionnaireAnswerRule.Optional };
        section.Questions.Add(first);
        section.Questions.Add(second);
        source.Sections.Add(section);
        source.Rules.Add(new QuestionnaireRule { Id = Guid.NewGuid(), QuestionnaireId = sourceId, SourceQuestionId = sourceQuestionId, TargetQuestionId = targetQuestionId, Operator = QuestionnaireRuleOperator.Equals, Action = QuestionnaireRuleAction.Show, Value = "YES" });
        return (source, QuestionnaireGraph.ToUpdateRequest(source));
    }
}
