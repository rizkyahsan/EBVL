using System.Text.Json;
using EBVL.FrontEnd.Logics.Modules.Administration.ApiCalls.GetApiCall;
using EBVL.FrontEnd.WebUi.Common.Services.Clipboard;
using EBVL.Shared.Dto.Modules.Administration.ApiCalls.GetApiCall;

namespace EBVL.FrontEnd.WebUi.Modules.Administration.Features.ApiCalls.Pages;

public partial class Details
{
    [Inject]
    public required ClipboardService ClipboardService { get; init; }

    [Parameter]
    public Guid Id { get; init; }

    private ApiCallItem _item = default!;
    private string _prettyResponseContent = default!;
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    protected override async Task OnParametersSetAsync()
    {
        _pageTitle = ApiCallsDisplayTextFor.ApiCall;

        LoadBreadcrumbs();
        await LoadItem();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            AdministrationBreadcrumbFor.Index,
            AdministrationApiCallsBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(_pageTitle)
        ];
    }

    private async Task LoadItem()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var query = new GetApiCallQuery
            {
                ApiCallId = Id
            };

            var response = await Sender.Send(query);

            _item = response.Item;
            _pageTitle = $"{_item.ServiceName} from {_item.ServiceProvider}";

            _breadcrumbItems.RemoveAt(_breadcrumbItems.Count - 1);
            _breadcrumbItems.Add(CommonBreadcrumbFor.Active(_pageTitle));

            if (string.IsNullOrWhiteSpace(_item.ResponseContent))
            {
                _prettyResponseContent = string.Empty;

                return;
            }

            using var jsonDocument = JsonDocument.Parse(_item.ResponseContent);
            _prettyResponseContent = JsonSerializer.Serialize(jsonDocument, _options);
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

    private async Task CopyToClipBoard(string content)
    {
        await ClipboardService.WriteTextAsync(content);

        Snackbar.AddSuccess($"The content has been copied to the clipboard.");
    }
}
