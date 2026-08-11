using EBVL.FrontEnd.Logics.Modules.Administration.Configurations.DeleteConfiguration;
using EBVL.FrontEnd.Logics.Modules.Administration.Configurations.GetConfigurations;
using EBVL.FrontEnd.Logics.Modules.Administration.Configurations.UpdateConfiguration;
using EBVL.FrontEnd.WebUi.Modules.Administration.Features.Configurations.Components;
using EBVL.Shared.Dto.Modules.Administration.Configurations.GetConfigurations;

namespace EBVL.FrontEnd.WebUi.Modules.Administration.Features.Configurations.Pages;

public partial class Index
{
    private IEnumerable<ConfigurationItem> _items = [];
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
            AdministrationBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(ConfigurationsDisplayTextFor.Configurations)
        ];
    }

    private async Task LoadItems()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var query = new GetConfigurationsQuery();
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

    private bool FilterItems(ConfigurationItem item)
    {
        if (string.IsNullOrWhiteSpace(_searchKeyword))
        {
            return true;
        }

        if (item.Key.Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        if (item.Value.Contains(_searchKeyword, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        return false;
    }

    private async Task ShowDialogAdd()
    {
        ClearException();

        var dialog = await DialogService.ShowAsync<DialogAdd>($"{CommonDisplayTextFor.Add} {ConfigurationsDisplayTextFor.Configuration}");
        var result = await dialog.Result;

        if (result is not null && !result.Canceled)
        {
            await LoadItems();
        }
    }

    private async Task ShowDialogEdit(ConfigurationItem item)
    {
        ClearException();

        var command = new UpdateConfigurationCommand
        {
            ConfigurationId = item.Id,
            Key = item.Key,
            Value = item.Value
        };

        var parameters = new DialogParameters<DialogEdit>
        {
            { x => x.Model, command }
        };

        var dialog = await DialogService.ShowAsync<DialogEdit>($"{CommonDisplayTextFor.Edit} {ConfigurationsDisplayTextFor.Configuration}", parameters);
        var result = await dialog.Result;

        if (result is not null && !result.Canceled)
        {
            await LoadItems();
        }
    }

    private async Task ShowDialogDelete(ConfigurationItem item)
    {
        var dialogResult = await DialogService.ShowMessageBox(
            $"{CommonDisplayTextFor.Delete} {ConfigurationsDisplayTextFor.Configuration}",
            ConfirmationMessageFor.Delete(ConfigurationsDisplayTextFor.Configuration, item.Key),
            yesText: CommonDisplayTextFor.Yes,
            noText: CommonDisplayTextFor.No,
            options: new DialogOptions { MaxWidth = MaxWidth.ExtraSmall });

        if (dialogResult is true)
        {
            try
            {
                _isLoading = true;

                ClearException();

                var command = new DeleteConfigurationCommand
                {
                    ConfigurationId = item.Id
                };

                await Sender.Send(command);

                Snackbar.AddSuccess(SuccessMessageFor.Deleted(ConfigurationsDisplayTextFor.Configuration, item.Key));

                await LoadItems();
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
