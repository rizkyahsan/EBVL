using EBVL.FrontEnd.Logics.Modules.MasterData.Countries.UpdateCountry;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Countries.Components;

public partial class DrawerEdit
{
    [Parameter, EditorRequired]
    public EventCallback OnReload { get; set; }

    [Parameter, EditorRequired]
    public required UpdateCountryCommand? Model { get; init; }

    private MudForm _form = default!;
    private UpdateCountryCommandValidator _validator = default!;

    protected override void OnInitialized()
    {
        _validator = new();
    }

    private async Task Submit()
    {
        ClearException();

        if (Model is null)
        {
            return;
        }

        try
        {
            _isLoading = true;

            await _form.RunValidation();
            await Sender.Send(Model);

            Snackbar.AddSuccess(SuccessMessageFor.Updated(CountriesDisplayTextFor.Country, Model.Name));

            await IsOpenChanged.InvokeAsync(false);
            await OnReload.InvokeAsync();
        }
        catch (Exception exception)
        {
            _exception = exception;
            Snackbar.AddErrors(_exception.GetAllErrorMessages());
        }
        finally
        {
            _isLoading = false;
        }
    }
}
