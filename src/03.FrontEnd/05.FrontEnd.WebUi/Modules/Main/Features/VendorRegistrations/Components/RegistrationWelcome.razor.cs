namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;

public partial class RegistrationWelcome
{
    [Parameter]
    public required EventCallback OnBack { get; init; }

    [Parameter]
    public required EventCallback OnRegister { get; init; }
}
