using MediatR;

namespace EBVL.FrontEnd.WebUi.Common.Components.Abstracts;

public abstract class SelectorBase<TValue> : ComponentBase
{
    [Inject]
    public required ISender Sender { get; init; }

    [Inject]
    public required ISnackbar Snackbar { get; init; }

    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationStateTask { get; set; } = default!;

    [Parameter]
    public bool ShowAllOption { get; set; } = false;

    [Parameter]
    public bool IsAllowNoValue { get; set; } = true;

    [Parameter]
    public bool Readonly { get; set; } = false;

    [Parameter]
    public bool Required { get; set; } = false;

    [Parameter]
    public string RequiredError { get; set; } = "Required!";

    [Parameter]
    public TValue? Value { get; set; }

    [Parameter]
    public EventCallback<TValue> ValueChanged { get; set; }

    protected async Task OnValueChanged(TValue value)
    {
        if (EqualityComparer<TValue>.Default.Equals(Value, value))
        {
            return;
        }

        Value = value;

        await ValueChanged.InvokeAsync(value);
    }
}
