using EBVL.FrontEnd.Logics.Modules.MasterData.Lenders.AddLender;
using EBVL.Shared.Dto.Modules.MasterData.Countries.GetCountries;

namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Lenders.Components;

public partial class DrawerAdd
{
    [Parameter, EditorRequired]
    public EventCallback OnReload { get; set; }

    private MudForm _form = default!;
    private AddLenderCommand _model = default!;
    private AddLenderCommandValidator _validator = default!;
    private string _countryPhoneCode = string.Empty;

    protected override void OnInitialized()
    {
        _model = new()
        {
            Name = string.Empty,
            Address = string.Empty,
            CountryId = Guid.Empty,
            PhoneNumber = string.Empty,
            EmailAddress = string.Empty,
            Website = string.Empty
        };

        _validator = new();
    }

    private void HandleCountrySelected(CountryItem? country)
    {
        _countryPhoneCode = country?.PhoneCode ?? string.Empty;
    }

    private async Task Submit()
    {
        ClearException();

        try
        {
            _isLoading = true;

            await _form.RunValidation();
            _ = await Sender.Send(_model);

            Snackbar.AddSuccess(SuccessMessageFor.Added(LendersDisplayTextFor.Lender, _model.Name));

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
