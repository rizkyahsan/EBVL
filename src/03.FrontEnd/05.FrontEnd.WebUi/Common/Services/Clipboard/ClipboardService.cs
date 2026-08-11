using Microsoft.JSInterop;

namespace EBVL.FrontEnd.WebUi.Common.Services.Clipboard;

public sealed class ClipboardService(IJSRuntime jsRuntime)
{
    public ValueTask WriteTextAsync(string text)
    {
        return jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
    }
}
