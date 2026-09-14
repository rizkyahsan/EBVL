using EBVL.FrontEnd.Logics.Modules.MasterData.Documents;
using EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Documents.Components;
using EBVL.Shared.Dto.Modules.MasterData.Documents;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Documents.Pages;

public partial class Index
{
    private List<DocumentListItem> _items = [];
    private string? _search;

    protected override async Task OnInitializedAsync()
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
            Statics.BreadcrumbFor.Index
        ];
    }

    private async Task Load()
    {
        try
        {
            _isLoading = true;
            ClearException();
            _items = [.. (await Sender.Send(new GetDocumentsQuery())).Items];
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

    private bool Filter(DocumentListItem item)
    {
        return string.IsNullOrWhiteSpace(_search)
            || $"{item.BusinessProcess} {item.Name} {item.MaxSizeMb}".Contains(_search, StringComparison.OrdinalIgnoreCase);
    }

    private int Number(DocumentListItem item)
    {
        return _items.IndexOf(item) + 1;
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
        if (document is not null)
        {
            document = (await Sender.Send(new GetDocumentQuery(document.Id))).Item;
        }

        var parameters = new DialogParameters<DialogDocument>();
        if (document is not null)
        {
            parameters.Add(component => component.Document, document);
        }

        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true };
        var title = document is null ? "Add New Document" : "Edit Document";
        var dialog = await DialogService.ShowAsync<DialogDocument>(title, parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false })
        {
            _search = null;
            await Load();
        }
    }
}
