namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;

public partial class ReviewField
{
    [Parameter]
    public required string Label { get; init; }

    [Parameter]
    public string? Value { get; init; }

    [Parameter]
    public bool IsLink { get; init; }
}
