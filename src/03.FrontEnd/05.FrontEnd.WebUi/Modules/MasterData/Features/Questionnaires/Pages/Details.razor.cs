using EBVL.FrontEnd.Logics.Modules.MasterData.Questionnaires;
using EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Components;
using EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Models;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

#pragma warning disable IDE0022, IDE0044, IDE0072
namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Pages;

public partial class Details
{
    [Parameter] public Guid Id { get; set; }
    [Parameter] public Guid SectionId { get; set; }

    private MudTable<QuestionnaireQuestionItem> _table = default!;
    private QuestionnaireItem? _item;
    private QuestionnaireSectionItem? _section;
    private string? _search;
    private bool _busy;
    private int _page;
    private int _pageSize = 10;
    private IReadOnlyList<QuestionnaireQuestionItem> _currentRows = [];

    #region Lifecycle

    protected override async Task OnParametersSetAsync()
    {
        LoadBreadcrumbs();
        await LoadHeader();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems = [MainBreadcrumbFor.Home, MasterDataBreadcrumbFor.Index, new BreadcrumbItem("Questionnaire", QuestionnaireRouteFor.Index), CommonBreadcrumbFor.Active("Detail Section")];
    }

    private async Task LoadHeader()
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

    #endregion

    #region Table

    private async Task<TableData<QuestionnaireQuestionItem>> ReloadTable(TableState state, CancellationToken token)
    {
        _page = state.Page;
        _pageSize = state.PageSize;
        if (_item is null || _section is null)
        {
            return new TableData<QuestionnaireQuestionItem>();
        }

        try
        {
            _isLoading = true;
            ClearException();
            var query = new GetQuestionnaireQuestionsQuery(_item.QuestionnaireId, _section.Id)
            {
                Page = state.Page + 1,
                PageSize = state.PageSize,
                SearchText = _search,
                SortField = state.SortLabel,
                SortOrder = state.SortDirection switch
                {
                    SortDirection.Ascending => Pertamina.Common.Dto.Enums.SortOrder.Ascending,
                    SortDirection.Descending => Pertamina.Common.Dto.Enums.SortOrder.Descending,
                    _ => null
                }
            };
            var response = await Sender.Send(query, token);
            _currentRows = response.Items.ToList();
            return response.ToTableData();
        }
        catch (Exception exception)
        {
            _exception = exception;
            return new TableData<QuestionnaireQuestionItem>();
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task OnSearch(string value)
    {
        _search = value.Trim();
        await _table.ReloadServerData();
    }

    #endregion

    #region Question Actions

    private Task AddQuestion() => OpenQuestion(null);
    private Task EditQuestion(QuestionnaireQuestionItem question) => OpenQuestion(question);

    private async Task OpenQuestion(QuestionnaireQuestionItem? question)
    {
        var model = question is null ? null : ToModel(question);
        var parameters = new DialogParameters<DialogQuestion>
        {
            { x => x.Model, model },
            { x => x.QuestionnaireId, Id },
            { x => x.SectionId, SectionId },
            { x => x.NextOrder, Math.Max(1, (_section?.Questions.Count ?? 0) + 1) },
            { x => x.OnSubmit, EventCallback.Factory.Create<QuestionModel>(this, changed => SaveQuestion(changed, question?.Code)) }
        };
        var title = question is null ? "Add Questionnaire" : "Edit Questionnaire";
        var dialog = await DialogService.ShowAsync<DialogQuestion>(title, parameters, new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, CloseButton = true });
        _ = await dialog.Result;
    }

    private async Task SaveQuestion(QuestionModel model, string? stableCode)
    {
        if (_item is null || _section is null || _busy)
        {
            return;
        }

        try
        {
            _busy = true;
            var sectionCode = _section.Code;
            var item = await EditableItem();
            var section = item.Sections.Single(x => x.Code == sectionCode);
            if (stableCode is null)
            {
                item = (await Sender.Send(new AddQuestionnaireQuestionCommand(item.QuestionnaireId, section.Id, new()
                {
                    Code = model.Code,
                    Label = model.Label,
                    Hint = model.Hint,
                    Placeholder = model.Placeholder,
                    Type = model.Type,
                    VendorType = model.CompanyType,
                    AnswerRule = model.AnswerRule,
                    Order = model.Order,
                    IsVisible = model.IsVisible,
                    IsActive = model.IsActive,
                    Options = [.. model.Options.Select((x, index) => new QuestionnaireOptionRequest(null, x.Code, x.Label, index + 1))],
                    RowVersion = item.RowVersion
                }))).Item;
                Snackbar.AddSuccess("Questionnaire saved.");
                await NavigateAndRefresh(item, sectionCode);
                return;
            }

            var question = section.Questions.Single(x => x.Code == stableCode);
            item = (await Sender.Send(new UpdateQuestionnaireQuestionCommand(item.QuestionnaireId, section.Id, question.Id, ToRequest(model, item.RowVersion)))).Item;
            Snackbar.AddSuccess("Questionnaire saved.");
            await NavigateAndRefresh(item, sectionCode);
        }
        catch (Exception exception)
        {
            _exception = exception;
            throw;
        }
        finally
        {
            _busy = false;
        }
    }

    private async Task DeleteQuestion(QuestionnaireQuestionItem question)
    {
        if (_item?.Status != QuestionnaireStatus.Draft || _section is null || _busy)
        {
            return;
        }

        var confirmed = await DialogService.ShowMessageBox("Delete Questionnaire", $"Delete '{question.Label}'?", yesText: "Delete", cancelText: "Cancel");
        if (confirmed != true)
        {
            return;
        }

        try
        {
            _busy = true;
            var item = (await Sender.Send(new GetQuestionnaireQuery(_item.QuestionnaireId))).Item;
            var sectionCode = _section.Code;
            item = (await Sender.Send(new DeleteQuestionnaireQuestionCommand(item.QuestionnaireId, _section.Id, question.Id, item.RowVersion))).Item;
            Snackbar.AddSuccess("Questionnaire deleted.");
            await NavigateAndRefresh(item, sectionCode);
        }
        catch (Exception exception)
        {
            _exception = exception;
        }
        finally
        {
            _busy = false;
        }
    }

    #endregion

    #region Helpers

    private async Task<QuestionnaireItem> EditableItem()
    {
        var item = (await Sender.Send(new GetQuestionnaireQuery(Id))).Item;
        if (item.Status == QuestionnaireStatus.Publish)
        {
            item = (await Sender.Send(new CreateQuestionnaireDraftCommand(item.QuestionnaireId))).Item;
        }

        if (item.Status != QuestionnaireStatus.Draft)
        {
            throw new InvalidOperationException("Historical questionnaire versions are read-only.");
        }

        return item;
    }

    private async Task NavigateAndRefresh(QuestionnaireItem item, string sectionCode)
    {
        _item = item;
        _section = item.Sections.Single(x => x.Code == sectionCode);
        Id = item.QuestionnaireId;
        SectionId = _section.Id;
        NavigationManager.NavigateTo(QuestionnaireRouteFor.Details(Id, SectionId), replace: true);
        await _table.ReloadServerData();
    }

    private static UpdateQuestionnaireQuestionRequest ToRequest(QuestionModel model, string rowVersion) => new()
    {
        Code = model.Code,
        Label = model.Label,
        Hint = model.Hint,
        Placeholder = model.Placeholder,
        Type = model.Type,
        VendorType = model.CompanyType,
        Order = model.Order,
        AnswerRule = model.AnswerRule,
        IsVisible = model.IsVisible,
        IsActive = model.IsActive,
        Options = [.. model.Options.Select((x, index) => new QuestionnaireOptionRequest(null, x.Code, x.Label, index + 1))],
        RowVersion = rowVersion
    };

    private static QuestionModel ToModel(QuestionnaireQuestionItem question) => new()
    {
        Id = question.Id,
        Code = question.Code,
        Label = question.Label,
        Hint = question.Hint,
        Placeholder = question.Placeholder,
        Type = question.Type,
        CompanyType = question.CompanyType,
        IsRequired = question.IsRequired,
        IsVisible = question.IsVisible,
        IsActive = question.IsActive,
        AnswerRule = question.AnswerRule,
        Order = question.Order,
        Options = [.. question.Options.OrderBy(x => x.Order).Select(x => new OptionModel { Code = x.Code, Label = x.Label })]
    };

    private int Number(QuestionnaireQuestionItem item) => (_page * _pageSize) + _currentRows.ToList().IndexOf(item) + 1;
    private static string AnswerType(QuestionnaireQuestionType value) => value switch
    {
        QuestionnaireQuestionType.ShortText => "Textbox",
        QuestionnaireQuestionType.LongText => "Textarea",
        QuestionnaireQuestionType.File => "Upload File",
        _ => value.ToString()
    };
    private static string AnswerRule(QuestionnaireAnswerRule value) => value switch
    {
        QuestionnaireAnswerRule.Mandatory => "Mandatory (*)",
        QuestionnaireAnswerRule.AddedValue => "Added Value (**)",
        _ => "Optional"
    };

    #endregion
}
