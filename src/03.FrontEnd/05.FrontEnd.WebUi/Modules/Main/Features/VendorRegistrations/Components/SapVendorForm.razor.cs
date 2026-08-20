using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.SapVendor;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;

public partial class SapVendorForm
{
    [Parameter]
    public required SapVendorRequest Model { get; init; }

    [Parameter]
    public required EventCallback OnBack { get; init; }

    [Parameter]
    public required EventCallback<SapVendorRequest> OnNext { get; init; }

    private MudForm _form = default!;
    private readonly SapVendorRequestValidator _validator = new();

    private async Task Submit()
    {
        await _form.Validate();

        if (!_form.IsValid)
        {
            return;
        }

        await OnNext.InvokeAsync(Model);
    }
}
