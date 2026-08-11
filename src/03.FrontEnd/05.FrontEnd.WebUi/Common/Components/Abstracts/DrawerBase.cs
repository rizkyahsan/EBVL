namespace EBVL.FrontEnd.WebUi.Common.Components.Abstracts;

public abstract class DrawerBase : CommonComponentBase
{
    [Parameter]
    public bool IsOpen { get; set; }

    [Parameter]
    public EventCallback<bool> IsOpenChanged { get; set; }

    protected virtual async Task CloseDrawer()
    {
        IsOpen = false;

        await IsOpenChanged.InvokeAsync(false);
    }
}
