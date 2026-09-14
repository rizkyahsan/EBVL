using EBVL.FrontEnd.Logics.Modules.MasterData.Questionnaires;
using EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Components;
using EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Models;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Pages;

public partial class Details
{
    [Parameter] public Guid Id { get; set; }
    [Parameter] public Guid SectionId { get; set; }

    private QuestionnaireItem? _item;
    private QuestionnaireSectionItem? _section;
    private string? _search;

    protected override async Task OnParametersSetAsync()
    {
        LoadBreadcrumbs();
        await Load();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            MasterDataBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active("Questionnaire Detail")
        ];
    }

    private async Task Load()
    {
        try
        {
            _isLoading = true;
            ClearException();
            _item = (await Sender.Send(new GetQuestionnaireQuery(Id))).Item;
            _section = _item.Sections.Single(x => x.Id == SectionId);
        }
        catch (Exception exception)
        {
            _exception = exception;
        }
        finally
        {
            _isLoading = false;
        }
    }

    private Task AddQuestion()
    {
        return OpenQuestion(null);
    }

    private Task EditQuestion(QuestionnaireQuestionItem question)
    {
        return OpenQuestion(question);
    }

    private async Task OpenQuestion(QuestionnaireQuestionItem? question)
    {
        if (_item is null || _section is null)
        {
            return;
        }

        var model = question is null ? null : ToModel(question);
        var parameters = new DialogParameters<DialogQuestion>
        {
            { x => x.Model, model },
            { x => x.QuestionnaireId, Id },
            { x => x.SectionId, SectionId },
            { x => x.NextOrder, _section.Questions.Count + 1 },
            { x => x.OnSubmit, EventCallback.Factory.Create<QuestionModel>(this, SaveQuestion) }
        };
        var options = new DialogOptions
        {
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
            CloseButton = true
        };
        var title = question is null ? "Add Questionnaire" : "Edit Questionnaire";
        var dialog = await DialogService.ShowAsync<DialogQuestion>(title, parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: QuestionnaireItem updated })
        {
            _item = updated;
            _section = updated.Sections.Single(x => x.Id == SectionId);
            Snackbar.AddSuccess("Questionnaire saved.");
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task SaveQuestion(QuestionModel changed)
    {
        if (_item is null || _section is null)
        {
            return;
        }

        try
        {
            var sectionCode = _section.Code;
            var sections = _item.Sections
                .Select(section => new QuestionnaireSectionItem(
                    section.Id,
                    section.Code,
                    section.Title,
                    section.Order,
                    section.CompanyType,
                    section.IsActive,
                    section.Id == SectionId ? Apply(section.Questions, changed) : section.Questions))
                .ToList();
            var request = new UpdateQuestionnaireRequest
            {
                BusinessProcess = _item.BusinessProcess,
                IsActive = _item.IsActive,
                RowVersion = _item.RowVersion,
                Sections = sections,
                Rules = _item.Rules.ToList()
            };
            _item = (await Sender.Send(new UpdateQuestionnaireCommand(Id, request))).Item;
            _section = _item.Sections.Single(x => x.Code == sectionCode);
            SectionId = _section.Id;
            Snackbar.AddSuccess("Questionnaire saved.");
            NavigationManager.NavigateTo(QuestionnaireRouteFor.Details(Id, SectionId), replace: true);
        }
        catch (Exception exception)
        {
            _exception = exception;
            throw;
        }
    }

    private static IReadOnlyList<QuestionnaireQuestionItem> Apply(
        IReadOnlyList<QuestionnaireQuestionItem> questions,
        QuestionModel changed)
    {
        var rows = questions.Where(x => x.Id != changed.Id).OrderBy(x => x.Order).ToList();
        var options = changed.Options
            .Select((x, i) => new QuestionnaireOptionItem(Guid.Empty, x.Code, x.Label, i + 1))
            .ToList();
        var item = new QuestionnaireQuestionItem(
            changed.Id,
            changed.Code,
            changed.Label,
            changed.Hint,
            changed.Placeholder,
            changed.Type,
            null,
            changed.Order,
            changed.AnswerRule == QuestionnaireAnswerRule.Mandatory,
            changed.IsVisible,
            changed.IsActive,
            changed.AnswerRule,
            options);

        rows.Insert(Math.Clamp(changed.Order - 1, 0, rows.Count), item);
        return rows.Select((row, index) => row with { Order = index + 1 }).ToList();
    }

    private static QuestionModel ToModel(QuestionnaireQuestionItem question)
    {
        return new QuestionModel
        {
            Id = question.Id,
            Code = question.Code,
            Label = question.Label,
            Hint = question.Hint,
            Placeholder = question.Placeholder,
            Type = question.Type,
            IsRequired = question.IsRequired,
            IsVisible = question.IsVisible,
            IsActive = question.IsActive,
            AnswerRule = question.AnswerRule,
            Order = question.Order,
            Options = question.Options
                .Select(x => new OptionModel { Code = x.Code, Label = x.Label })
                .ToList()
        };
    }

    private bool Filter(QuestionnaireQuestionItem question)
    {
        return string.IsNullOrWhiteSpace(_search)
            || $"{question.Label} {question.Hint} {AnswerType(question.Type)} {AnswerRule(question.AnswerRule)} {question.Order} {(question.IsActive ? "Yes" : "No")}".Contains(_search, StringComparison.OrdinalIgnoreCase);
    }

    private int Number(QuestionnaireQuestionItem question)
    {
        return _section is null
            ? 0
            : _section.Questions.OrderBy(x => x.Order).ToList().IndexOf(question) + 1;
    }

    private static string AnswerType(QuestionnaireQuestionType value)
    {
        return value switch
        {
            QuestionnaireQuestionType.ShortText => "Textbox",
            QuestionnaireQuestionType.LongText => "Textarea",
            QuestionnaireQuestionType.Boolean => value.ToString(),
            QuestionnaireQuestionType.Integer => value.ToString(),
            QuestionnaireQuestionType.Decimal => value.ToString(),
            QuestionnaireQuestionType.Date => value.ToString(),
            QuestionnaireQuestionType.SingleChoice => value.ToString(),
            QuestionnaireQuestionType.MultipleChoice => value.ToString(),
            QuestionnaireQuestionType.Address => "Address",
            QuestionnaireQuestionType.File => "Upload File",
            _ => value.ToString()
        };
    }

    private static string AnswerRule(QuestionnaireAnswerRule value)
    {
        return value switch
        {
            QuestionnaireAnswerRule.Mandatory => "Mandatory (*)",
            QuestionnaireAnswerRule.AddedValue => "Added Value (**)",
            QuestionnaireAnswerRule.Optional => "Optional",
            _ => "Optional"
        };
    }
}
