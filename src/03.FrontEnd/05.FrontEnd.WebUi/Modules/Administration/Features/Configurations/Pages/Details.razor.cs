using EBVL.FrontEnd.Logics.Modules.Administration.Configurations.DeleteConfiguration;
using EBVL.FrontEnd.Logics.Modules.Administration.Configurations.GetConfiguration;
using EBVL.FrontEnd.Logics.Modules.Administration.Configurations.UpdateConfiguration;
using EBVL.FrontEnd.WebUi.Modules.Administration.Features.Configurations.Components;
using EBVL.Shared.Dto.Modules.Administration.Configurations.GetConfiguration;

namespace EBVL.FrontEnd.WebUi.Modules.Administration.Features.Configurations.Pages;

public partial class Details
{
    [Parameter]
    public Guid Id { get; init; }

    private ConfigurationItem _item = default!;

    protected override async Task OnParametersSetAsync()
    {
        _pageTitle = ConfigurationsDisplayTextFor.Configuration;

        LoadBreadcrumbs();
        await LoadItem();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            AdministrationBreadcrumbFor.Index,
            AdministrationConfigurationsBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(_pageTitle)
        ];
    }

    private async Task LoadItem()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var query = new GetConfigurationQuery
            {
                ConfigurationId = Id
            };

            var response = await Sender.Send(query);

            _item = response.Item;
            _pageTitle = _item.Key;

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
            await LoadItem();
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

                var command = new DeleteConfigurationCommand
                {
                    ConfigurationId = item.Id
                };

                await Sender.Send(command);

                Snackbar.AddSuccess(SuccessMessageFor.Deleted(ConfigurationsDisplayTextFor.Configuration, item.Key));

                NavigationManager.NavigateTo(AdministrationConfigurationsRouteFor.Index);
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
