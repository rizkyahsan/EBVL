using EBVL.FrontEnd.Logics.Modules.Examples.Documents.GetDocuments;
using EBVL.FrontEnd.WebUi.Modules.Examples.Features.Documents.Components;
using EBVL.Shared.Dto.Modules.Examples.Documents.GetDocuments;

namespace EBVL.FrontEnd.WebUi.Modules.Examples.Features.Documents.Pages;

public partial class Index
{
    private IEnumerable<DocumentItem> _items = [];
    private string? _searchKeyword;

    protected override async Task OnInitializedAsync()
    {
        LoadBreadcrumbs();
        await LoadItems();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            ExamplesBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(DocumentsDisplayTextFor.Documents)
        ];
    }

    private async Task LoadItems()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var query = new GetDocumentsQuery();
            var response = await Sender.Send(query);

            _items = response.Items;
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

    private bool FilterItems(DocumentItem item)
    {
        if (string.IsNullOrWhiteSpace(_searchKeyword))
        {
            return true;
        }

        if (item.FileName.Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        if (item.FileSize.ToReadableFileSize().Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        if (item.Created.ToDisplayText(DateTimeFormatFor.LongDateTime).Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        if (item.CreatedBy.Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        return false;
    }

    private async Task ShowDialogAdd()
    {
        var model = new DialogAddModel
        {
            Description = $"{DocumentsDisplayTextFor.Document} {DateTime.Now:d MMMM yyyy HH:mm:ss}"
        };

        var parameters = new DialogParameters<DialogAdd>
        {
            { x => x.Model, model }
        };

        var dialog = await DialogService.ShowAsync<DialogAdd>($"{CommonDisplayTextFor.Add} {DocumentsDisplayTextFor.Document}", parameters);
        var result = await dialog.Result;

        if (result is not null && !result.Canceled && result.Data is not null)
        {
            var documentId = (Guid)result.Data;

            NavigationManager.NavigateTo(ExamplesDocumentsRouteFor.Details(documentId));
        }
    }
}
