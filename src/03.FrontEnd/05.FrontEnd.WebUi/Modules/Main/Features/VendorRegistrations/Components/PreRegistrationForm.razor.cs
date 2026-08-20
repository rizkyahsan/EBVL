using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.PreRegistration;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;

public partial class PreRegistrationForm
{
    [Parameter]
    public required PreRegistrationRequest Model { get; init; }

    [Parameter]
    public required EventCallback OnBack { get; init; }

    [Parameter]
    public required EventCallback<PreRegistrationRequest> OnNext { get; init; }

    private void AddBrand()
    {
        Model.AdditionalBrands.Add(string.Empty);
    }

    private void UpdateBrand(int index, string? value)
    {
        Model.AdditionalBrands[index] = value ?? string.Empty;
    }

    private void RemoveBrand(int index)
    {
        Model.AdditionalBrands.RemoveAt(index);
    }

    private Task Submit()
    {
        return OnNext.InvokeAsync(Model);
    }
}
