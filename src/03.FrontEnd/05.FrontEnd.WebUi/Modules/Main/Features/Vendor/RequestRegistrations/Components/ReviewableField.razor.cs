namespace EBVL.FrontEnd.WebUi.Modules.Main.Features.Vendor.RequestRegistrations.Components;

public partial class ReviewableField : ComponentBase
{
    [Parameter, EditorRequired]
    public required string Label { get; init; }

    [Parameter, EditorRequired]
    public required string Value { get; init; }

    [Parameter]
    public bool IsFile { get; init; }

    [Parameter]
    public bool IsRequired { get; init; }

    [Parameter]
    public EventCallback OnView { get; init; }

    [Parameter]
    public bool? IsValid { get; set; } = true;

    [Parameter]
    public EventCallback<bool?> IsValidChanged { get; set; }

    [Parameter]
    public string? Remark { get; set; }

    [Parameter]
    public EventCallback<string?> RemarkChanged { get; set; }

    [Parameter]
    public bool ShowValidationErrors { get; init; }

    private async Task OnValidityChanged(bool? value)
    {
        await IsValidChanged.InvokeAsync(value);
        if (value is not false && !string.IsNullOrEmpty(Remark))
        {
            await RemarkChanged.InvokeAsync(null);
        }
    }

    private Task OnRemarkChanged(string? value)
    {
        return RemarkChanged.InvokeAsync(value);
    }
}
