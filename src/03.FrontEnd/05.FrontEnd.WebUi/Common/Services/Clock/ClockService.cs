namespace EBVL.FrontEnd.WebUi.Common.Services.Clock;

public sealed class ClockService : IDisposable
{
    private readonly PeriodicTimer _timer;
    public event Action? Tick;
    public ClockService()
    {
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        _ = RunAsync();
    }
    private async Task RunAsync()
    {
        while (await _timer.WaitForNextTickAsync())
        {
            Tick?.Invoke();
        }
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}
