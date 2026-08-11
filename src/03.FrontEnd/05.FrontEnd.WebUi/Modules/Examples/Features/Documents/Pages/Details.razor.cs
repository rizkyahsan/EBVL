using EBVL.FrontEnd.Logics.Modules.Examples.Documents.DeleteDocument;
using EBVL.FrontEnd.Logics.Modules.Examples.Documents.GetDocument;
using EBVL.FrontEnd.WebUi.Modules.Examples.Features.Documents.Components;
using EBVL.Shared.Dto.Modules.Examples.Documents.GetDocument;

namespace EBVL.FrontEnd.WebUi.Modules.Examples.Features.Documents.Pages;

public partial class Details
{
    [Parameter]
    public Guid Id { get; init; }

    private DocumentItem _item = default!;

    protected override async Task OnParametersSetAsync()
    {
        _pageTitle = DocumentsDisplayTextFor.Document;

        LoadBreadcrumbs();
        await LoadItem();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            ExamplesBreadcrumbFor.Index,
            ExamplesDocumentsBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(_pageTitle)
        ];
    }

    private async Task LoadItem()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var query = new GetDocumentQuery
            {
                DocumentId = Id
            };

            var response = await Sender.Send(query);

            _item = response.Item;
            _pageTitle = _item.FileName;

            _breadcrumbItems.RemoveAt(_breadcrumbItems.Count - 1);
            _breadcrumbItems.Add(CommonBreadcrumbFor.Active(_pageTitle));
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

    private async Task ShowDialogEdit()
    {
        ClearException();

        var model = new DialogEditModel
        {
            DocumentId = _item.Id,
            FileName = _item.FileName,
            Description = _item.Description
        };

        var parameters = new DialogParameters<DialogEdit>
        {
            { x => x.Model, model }
        };

        var dialog = await DialogService.ShowAsync<DialogEdit>($"{CommonDisplayTextFor.Edit} {DocumentsDisplayTextFor.Document}", parameters);
        var result = await dialog.Result;

        if (result is not null && !result.Canceled)
        {
            await LoadItem();
        }
    }

    private async Task ShowDialogDelete()
    {
        var dialogResult = await DialogService.ShowMessageBox(
            $"{CommonDisplayTextFor.Delete} {DocumentsDisplayTextFor.Document}",
            ConfirmationMessageFor.Delete(DocumentsDisplayTextFor.Document, _item.FileName),
            yesText: CommonDisplayTextFor.Yes,
            noText: CommonDisplayTextFor.No,
            options: new DialogOptions { MaxWidth = MaxWidth.ExtraSmall });

        if (dialogResult is true)
        {
            try
            {
                _isLoading = true;

                var command = new DeleteDocumentCommand
                {
                    DocumentId = _item.Id
                };

                await Sender.Send(command);

                Snackbar.AddSuccess(SuccessMessageFor.Deleted(DocumentsDisplayTextFor.Document, _item.FileName));

                NavigationManager.NavigateTo(ExamplesDocumentsRouteFor.Index);
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
    }
}
