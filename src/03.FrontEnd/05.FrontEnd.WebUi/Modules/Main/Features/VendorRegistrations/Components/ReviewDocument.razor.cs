using EBVL.Shared.Statics.VendorRegistrations;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.VendorRegistrations.Components;

public partial class ReviewDocument
{
    [Parameter]
    public required DocumentEvidenceDefinition Definition { get; init; }

    [Parameter]
    public string? FileName { get; init; }

    [Parameter]
    public required EventCallback<string> OnDownload { get; init; }

    private Task Download()
    {
        return OnDownload.InvokeAsync(Definition.Key);
    }
}
