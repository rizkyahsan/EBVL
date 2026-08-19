using EBVL.FrontEnd.Logics.Modules.MasterData.Lenders.UpdateLender;
using EBVL.Shared.Dto.Modules.MasterData.Countries.GetCountries;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Lenders.Components;

public partial class DrawerEdit
{
    [Parameter, EditorRequired]
    public EventCallback OnReload { get; set; }

    [Parameter, EditorRequired]
    public required UpdateLenderCommand? Model { get; init; }

    private MudForm _form = default!;
    private UpdateLenderCommandValidator _validator = default!;
    private string _countryPhoneCode = string.Empty;

    protected override void OnInitialized()
    {
        _validator = new();
    }

    private void HandleCountrySelected(CountryItem? country)
    {
        _countryPhoneCode = country?.PhoneCode ?? string.Empty;
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

            Snackbar.AddSuccess(SuccessMessageFor.Updated(LendersDisplayTextFor.Lender, Model.Name));

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
