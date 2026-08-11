using EBVL.FrontEnd.WebUi.Modules.Examples.Components;
using EBVL.FrontEnd.WebUi.Modules.Examples.Models;

namespace EBVL.FrontEnd.WebUi.Modules.Examples.Pages;

public partial class DisplayingInformation
{
    private MudForm _form1 = default!;
    private readonly Something _model1 = new() { Name = FakerFor.English.Name.FullName() };
    private readonly SomethingValidator _validator1 = new();
    private Exception? _exception1;

    private MudForm _form2 = default!;
    private readonly Something _model2 = new() { Name = FakerFor.English.Name.FullName() };
    private readonly SomethingValidator _validator2 = new();
    private Exception? _exception2;

    protected override void OnInitialized()
    {
        _pageTitle = ExamplesDisplayTextFor.DisplayingInformation;

        LoadBreadcrumbs();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            ExamplesBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(ExamplesDisplayTextFor.DisplayingInformation)
        ];
    }

    private static async Task<SomethingResult> AddSomething(MudForm form, Something something)
    {
        await form.RunValidation();
        await Task.Delay(500);

        var somethingResult = new SomethingResult
        {
            Name = something.Name,
            Code = Guid.CreateVersion7().ToString()
        };

        return somethingResult;
    }

    private async Task AddSomething1()
    {
        try
        {
            _exception1 = null;
            _isLoading = true;

            var somethingResult = await AddSomething(_form1, _model1);

            var parameters = new DialogParameters<DialogAddSomethingResult>
            {
                { x => x.SomethingResult, somethingResult }
            };

            _ = await DialogService.ShowAsync<DialogAddSomethingResult>("Add Something 1 Result", parameters);
        }
        catch (Exception exception)
        {
            _exception1 = exception;
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task AddSomething2()
    {
        try
        {
            _exception2 = null;
            _isLoading = true;

            var somethingResult = await AddSomething(_form2, _model2);
            var message = $"You have added Something 2 with Name {somethingResult.Name} and Code {somethingResult.Code}";

            _ = Snackbar.Add(message, MudBlazor.Severity.Info, options =>
            {
                options.ShowCloseIcon = true;
                options.RequireInteraction = true;
            });
        }
        catch (Exception exception)
        {
            _exception2 = exception;
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task ShowDialogAddSomething3()
    {
        var dialog = await DialogService.ShowAsync<DialogAddSomething>("Add Something 3");
        var result = await dialog.Result;

        if (result is not null && !result.Canceled && result.Data is not null)
        {
            var somethingResult = (SomethingResult)result.Data;

            var parameters = new DialogParameters<DialogAddSomethingResult>
            {
                { x => x.SomethingResult, somethingResult }
            };

            _ = await DialogService.ShowAsync<DialogAddSomethingResult>("Add Something 3 Result", parameters);
        }
    }

    private async Task ShowDialogAddSomething4()
    {
        var dialog = await DialogService.ShowAsync<DialogAddSomething>("Add Something 4");
        var result = await dialog.Result;

        if (result is not null && !result.Canceled && result.Data is not null)
        {
            var somethingResult = (SomethingResult)result.Data;
            var message = $"You have added Something 4 with Name {somethingResult.Name} and Code {somethingResult.Code}";

            _ = Snackbar.Add(message, MudBlazor.Severity.Info, options =>
            {
                options.ShowCloseIcon = true;
                options.RequireInteraction = true;
            });
        }
    }
}
