namespace EBVL.FrontEnd.WebUi.Common.Components;

public partial class Countdown : ComponentBase, IDisposable
{
    [Parameter]
    public DateTimeOffset? DueDate { get; set; }

    private PeriodicTimer? _timer;
    private CancellationTokenSource? _cts;

    private string _displayText = "-";
    private Color _color = Color.Success;

    protected override async Task OnInitializedAsync()
    {
        _cts = new CancellationTokenSource();
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        _ = RunTimerAsync(_cts.Token);

        await base.OnInitializedAsync();
    }

    protected override void OnParametersSet()
    {
        UpdateDisplay();
    }

    private async Task RunTimerAsync(CancellationToken token)
    {
        try
        {
            while (await _timer!.WaitForNextTickAsync(token))
            {
                UpdateDisplay();
                await InvokeAsync(StateHasChanged);
            }
        }
        catch (OperationCanceledException)
        {
            // Ignore
        }
    }

    private void UpdateDisplay()
    {
        if (DueDate is null)
        {
            _displayText = "-";
            _color = Color.Default;
            return;
        }

        var remaining = DueDate.Value.DateTime - DateTime.Now;

        if (remaining <= TimeSpan.Zero)
        {
            _displayText = "Overdue";
            _color = Color.Error;
            return;
        }

        _displayText = FormatRemaining(remaining);

        _color = remaining <= TimeSpan.FromDays(7) ? Color.Warning : Color.Info;
    }

    private static string FormatRemaining(TimeSpan remaining)
    {
        var parts = new List<string>();

        if (remaining.Days > 0)
        {
            parts.Add($"{remaining.Days}d");
        }

        if (remaining.Hours > 0)
        {
            parts.Add($"{remaining.Hours}h");
        }

        if (remaining.Minutes > 0)
        {
            parts.Add($"{remaining.Minutes}m");
        }

        parts.Add($"{remaining.Seconds}s");

        return $"{string.Join(" ", parts)} left";
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _timer?.Dispose();
        _cts?.Dispose();
    }
}
