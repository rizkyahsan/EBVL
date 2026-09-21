using EBVL.FrontEnd.Logics.Modules.MasterData.Questionnaires;
using EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Components;
using EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Models;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

#pragma warning disable IDE0022, IDE0044, IDE0072
namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Pages;

public partial class Index
{
    private MudTable<QuestionnaireSectionListItem> _table = default!;
    private List<QuestionnaireListItem> _questionnaires = [];
    private QuestionnaireListItem? _selected;
    private Guid? _selectedQuestionnaireId;
    private string? _search;
    private bool _busy;
    private int _page;
    private int _pageSize = 10;
    private IReadOnlyList<QuestionnaireSectionListItem> _currentRows = [];

    #region Lifecycle

    protected override async Task OnInitializedAsync()
    {
        LoadBreadcrumbs();
        await LoadQuestionnaires();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems = [MainBreadcrumbFor.Home, MasterDataBreadcrumbFor.Index, CommonBreadcrumbFor.Active("Questionnaire")];
    }

    #endregion

    #region Loading

    private async Task LoadQuestionnaires(Guid? preferredId = null)
    {
        try
        {
            _isLoading = true;
            ClearException();
            _questionnaires = [];
            _selected = null;
            _selectedQuestionnaireId = null;
            _questionnaires = [.. (await Sender.Send(new GetQuestionnairesQuery())).Items.OrderBy(x => x.BusinessProcess).ThenBy(x => x.Code)];
            _selected = _questionnaires.FirstOrDefault(x => x.QuestionnaireId == preferredId)
                ?? _questionnaires.FirstOrDefault(x => x.QuestionnaireId == _selectedQuestionnaireId)
                ?? _questionnaires.FirstOrDefault();
            _selectedQuestionnaireId = _selected?.QuestionnaireId;
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

    private async Task<TableData<QuestionnaireSectionListItem>> ReloadTable(TableState state, CancellationToken token)
    {
        _page = state.Page;
        _pageSize = state.PageSize;
        if (_selected is null)
        {
            return new TableData<QuestionnaireSectionListItem>();
        }

        try
        {
            _isLoading = true;
            ClearException();
            var query = new GetQuestionnaireSectionsQuery(_selected.QuestionnaireId)
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
            return new TableData<QuestionnaireSectionListItem>();
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task SelectQuestionnaire(Guid? id)
    {
        _selectedQuestionnaireId = id;
        _selected = _questionnaires.FirstOrDefault(x => x.QuestionnaireId == id);
        _search = null;
        await _table.ReloadServerData();
    }

    private string QuestionnaireText(Guid? id)
    {
        var questionnaire = _questionnaires.FirstOrDefault(x => x.QuestionnaireId == id);
        return questionnaire is null ? string.Empty : $"{questionnaire.BusinessProcess} ({Status(questionnaire.Status)}, v{questionnaire.Version})";
    }

    private async Task OnSearch(string value)
    {
        _search = value.Trim();
        await _table.ReloadServerData();
    }

    #endregion

    #region Section Actions

    private Task AddSection()
    {
        if (_selected is null)
        {
            return Task.CompletedTask;
        }

        return OpenSection(new SectionModel { BusinessProcess = _selected.BusinessProcess, Order = int.MaxValue });
    }

    private Task EditSection(QuestionnaireSectionListItem section)
    {
        return OpenSection(new SectionModel { Id = section.Id, BusinessProcess = section.BusinessProcess, Code = section.Code, Title = section.Title, CompanyType = section.VendorType, Order = section.Order, IsActive = section.IsActive }, section.Code);
    }

    private async Task OpenSection(SectionModel model, string? stableCode = null)
    {
        var parameters = new DialogParameters<DialogSection>
        {
            { x => x.Model, model },
            { x => x.OnSubmit, EventCallback.Factory.Create<SectionModel>(this, changed => SaveSection(changed, stableCode)) }
        };
        var dialog = await DialogService.ShowAsync<DialogSection>(model.Id == Guid.Empty ? "Add Section" : "Edit Section", parameters, new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, CloseButton = true });
        _ = await dialog.Result;
    }

    private async Task SaveSection(SectionModel model, string? stableCode)
    {
        if (_selected is null || _busy)
        {
            return;
        }

        try
        {
            _busy = true;
            var item = await EditableItem(_selected.QuestionnaireId);
            if (stableCode is null)
            {
                item = (await Sender.Send(new AddQuestionnaireSectionCommand(item.QuestionnaireId, new()
                {
                    BusinessProcess = model.BusinessProcess,
                    VendorType = model.CompanyType,
                    Code = model.Code,
                    Title = model.Title,
                    Order = model.Order,
                    IsActive = model.IsActive,
                    RowVersion = item.RowVersion
                }))).Item;
            }
            else
            {
                var section = item.Sections.Single(x => x.Code == stableCode);
                item = (await Sender.Send(new UpdateQuestionnaireSectionCommand(item.QuestionnaireId, section.Id, new()
                {
                    BusinessProcess = model.BusinessProcess,
                    VendorType = model.CompanyType,
                    Code = model.Code,
                    Section = model.Title,
                    Order = model.Order,
                    IsActive = model.IsActive,
                    RowVersion = item.RowVersion
                }))).Item;
            }

            Snackbar.AddSuccess("Questionnaire section saved.");
            await Refresh(item.QuestionnaireId);
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

    private async Task DeleteSection(QuestionnaireSectionListItem section)
    {
        if (_selected?.Status != QuestionnaireStatus.Draft || _busy)
        {
            return;
        }

        var confirmed = await DialogService.ShowMessageBox("Delete Section", $"Delete section '{section.Title}'?", yesText: "Delete", cancelText: "Cancel");
        if (confirmed != true)
        {
            return;
        }

        await Run(async () =>
        {
            var item = (await Sender.Send(new GetQuestionnaireQuery(_selected.QuestionnaireId))).Item;
            item = (await Sender.Send(new DeleteQuestionnaireSectionCommand(item.QuestionnaireId, section.Id, item.RowVersion))).Item;
            Snackbar.AddSuccess("Questionnaire section deleted.");
            await Refresh(item.QuestionnaireId);
        });
    }

    private async Task Publish()
    {
        if (_selected?.CanPublish != true || _busy)
        {
            return;
        }

        var confirmed = await DialogService.ShowMessageBox("Publish Questionnaire", $"Publish version {_selected.Version}?", yesText: "Publish", cancelText: "Cancel");
        if (confirmed != true)
        {
            return;
        }

        await Run(async () =>
        {
            var item = (await Sender.Send(new GetQuestionnaireQuery(_selected.QuestionnaireId))).Item;
            item = (await Sender.Send(new PublishQuestionnaireCommand(item.QuestionnaireId, item.RowVersion))).Item;
            Snackbar.AddSuccess($"Questionnaire version {item.Version} published.");
            await Refresh(item.QuestionnaireId);
        });
    }

    #endregion

    #region Helpers

    private async Task<QuestionnaireItem> EditableItem(Guid id)
    {
        var item = (await Sender.Send(new GetQuestionnaireQuery(id))).Item;
        if (item.Status == QuestionnaireStatus.Publish)
        {
            item = (await Sender.Send(new CreateQuestionnaireDraftCommand(id))).Item;
        }

        if (item.Status != QuestionnaireStatus.Draft)
        {
            throw new InvalidOperationException("Historical questionnaire versions are read-only.");
        }

        return item;
    }

    private async Task Refresh(Guid questionnaireId)
    {
        await LoadQuestionnaires(questionnaireId);
        await _table.ReloadServerData();
    }

    private async Task Run(Func<Task> action)
    {
        try
        {
            _busy = true;
            ClearException();
            await action();
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

    private int Number(QuestionnaireSectionListItem item) => (_page * _pageSize) + _currentRows.ToList().IndexOf(item) + 1;
    private static string Status(QuestionnaireStatus? status) => status?.ToString() ?? "-";
    private static string Vendor(VendorCompanyStatusType? value) => value switch
    {
        null => "All",
        VendorCompanyStatusType.Manufacture => "Vendor",
        VendorCompanyStatusType.SoleDistributorAgent => "Sole Agent",
        VendorCompanyStatusType.AuthorizedAgent => "Representative Office",
        _ => value.ToString()!
    };

    #endregion
}
