using EBVL.FrontEnd.Logics.Modules.MasterData.Countries.AddCountry;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Countries.Components;

public partial class DrawerAdd
{
    [Parameter, EditorRequired]
    public EventCallback OnReload { get; set; }

    private MudForm _form = default!;
    private AddCountryCommand _model = default!;
    private AddCountryCommandValidator _validator = default!;

    protected override void OnInitialized()
    {
        _model = new()
        {
            Code = string.Empty,
            Name = string.Empty,
            PhoneCode = string.Empty,
            CurrencyCode = string.Empty,
            Region = string.Empty
        };

        _validator = new();
    }

    private async Task Submit()
    {
        ClearException();

        try
        {
            _isLoading = true;

            await _form.RunValidation();
            _ = await Sender.Send(_model);

            Snackbar.AddSuccess(SuccessMessageFor.Added(CountriesDisplayTextFor.Country, _model.Name));

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
