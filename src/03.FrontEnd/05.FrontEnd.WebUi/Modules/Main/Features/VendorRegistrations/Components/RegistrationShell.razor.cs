namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;

public partial class RegistrationShell
{
    [Parameter]
    public required RenderFragment ChildContent { get; init; }

    [Parameter]
    public string? CardClass { get; init; }

    [Parameter]
    public bool Wide { get; init; }
}
