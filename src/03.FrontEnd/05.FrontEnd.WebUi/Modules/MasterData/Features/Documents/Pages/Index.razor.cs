using EBVL.FrontEnd.Logics.Modules.MasterData.Documents;
using EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Documents.Components;
using EBVL.Shared.Dto.Modules.MasterData.Documents;

#pragma warning disable IDE0072
namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Documents.Pages;

public partial class Index
{
    private List<DocumentRequirementSetListItem> _sets = [];
    private DocumentRequirementSetItem? _selected;
    private Guid? _selectedSetId;
    private string? _search;
    protected override async Task OnInitializedAsync()
    {
        LoadBreadcrumbs();
        await Load();
    }
    protected override void LoadBreadcrumbs() { _breadcrumbItems = [MainBreadcrumbFor.Home, MasterDataBreadcrumbFor.Index, Statics.BreadcrumbFor.Index]; }
    private async Task Load()
    {
        try
        {
            _isLoading = true;
            ClearException();
            _sets = [.. (await Sender.Send(new GetDocumentsQuery())).Items];
            _selectedSetId ??= _sets.FirstOrDefault()?.Id;

            if (_selectedSetId is { } id)
            {
                _selected = (await Sender.Send(new GetDocumentQuery(id))).Item;
            }
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
    private async Task SelectSet(Guid? id)
    {
        _selectedSetId = id;
        await Load();
    }
    private bool Filter(DocumentListItem item)
    {
        return string.IsNullOrWhiteSpace(_search) || item.Name.Contains(_search, StringComparison.OrdinalIgnoreCase);
    }

    private Task ShowAdd()
    {
        return ShowDialog(null);
    }

    private Task ShowEdit(DocumentListItem document)
    {
        return ShowDialog(document);
    }

    private async Task ShowDialog(DocumentListItem? document)
    {
        if (_selected is null)
        {
            return;
        }

        if (_selected.Status == QuestionnaireStatus.Publish)
        {
            var stableName = document?.Name;
            _selected = (await Sender.Send(new CreateDocumentDraftCommand(_selected.Id))).Item;
            _selectedSetId = _selected.Id;
            document = stableName is null ? null : _selected.Requirements.Single(x => x.Name == stableName);
        }

        var parameters = new DialogParameters<DialogDocument> { { x => x.RequirementSet, _selected }, { x => x.Document, document } };
        var dialog = await DialogService.ShowAsync<DialogDocument>(document is null ? "Add Document Requirement" : "Edit Document Requirement", parameters, new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true });
        if ((await dialog.Result) is { Canceled: false })
        {
            await Load();
        }
    }
    private async Task Publish()
    {
        if (_selected is null)
        {
            return;
        }

        _selected = (await Sender.Send(new PublishDocumentRequirementSetCommand(_selected.Id, _selected.RowVersion))).Item;
        await Load();
    }
    private async Task Delete(DocumentListItem document)
    {
        if (_selected is null || _selected.Status == QuestionnaireStatus.Superseded)
        {
            return;
        }

        var confirmed = await DialogService.ShowMessageBox("Delete Document", $"Delete '{document.Name}'?", yesText: "Delete", cancelText: "Cancel");
        if (confirmed != true)
        {
            return;
        }

        if (_selected.Status == QuestionnaireStatus.Publish)
        {
            _selected = (await Sender.Send(new CreateDocumentDraftCommand(_selected.Id))).Item;
            _selectedSetId = _selected.Id;
            document = _selected.Requirements.Single(x => x.Name == document.Name);
        }

        _selected = (await Sender.Send(new DeleteDocumentCommand(_selected.Id, document.Id, _selected.RowVersion))).Item;
        Snackbar.AddSuccess("Document deleted.");
        await Load();
    }

    private static Color StatusColor(QuestionnaireStatus? status)
    {
        return status switch
        {
            QuestionnaireStatus.Draft => Color.Warning,
            QuestionnaireStatus.Publish => Color.Success,
            QuestionnaireStatus.Superseded => Color.Default,
            null => Color.Default,
            _ => Color.Default
        };
    }
}
