using EBVL.Shared.Statics.Common;

namespace EBVL.FrontEnd.WebUi.Common.Components;

public partial class MyDateTimePicker
{
    [Parameter]
    public string DateLabel { get; set; } = "Select Date";

    [Parameter]
    public string DateFormat { get; set; } = "dd/MM/yyyy";

    [Parameter]
    public string TimeFormat { get; set; } = "HH:mm";

    [Parameter]
    public DateTime MinDate { get; set; }

    [Parameter]
    public bool Required { get; set; }

    [Parameter]
    public string RequiredError { get; set; } = "Required";

    [Parameter]
    public DateTimeOffset? Value { get; set; }

    [Parameter]
    public EventCallback<DateTimeOffset?> ValueChanged { get; set; }

    private DateTime? _date;
    private TimeSpan? _time;

    protected override void OnInitialized()
    {
        if (MinDate == default)
        {
            MinDate = TimeZoneInfo
                .ConvertTime(DateTimeOffset.UtcNow, TimezoneFor.WibTimeZone)
                .Date;
        }
    }

    protected override void OnParametersSet()
    {
        if (!Value.HasValue)
        {
            _date = null;
            _time = null;
            return;
        }

        // Stored in UTC
        var local = TimeZoneInfo.ConvertTime(Value.Value, TimezoneFor.WibTimeZone);

        _date = local.Date;
        _time = local.TimeOfDay;
    }

    private async Task OnDateChanged(DateTime? date)
    {
        _date = date;

        if (_date.HasValue && !_time.HasValue)
        {
            _time = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, TimezoneFor.WibTimeZone).TimeOfDay;
        }

        await UpdateValue();
    }

    private async Task OnTimeChanged(TimeSpan? time)
    {
        _time = time;

        if (_time.HasValue && !_date.HasValue)
        {
            _date = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, TimezoneFor.WibTimeZone).Date;
        }

        await UpdateValue();
    }

    private async Task UpdateValue()
    {
        DateTimeOffset? newValue = null;

        if (_date.HasValue && _time.HasValue)
        {
            var localDateTime =
                _date.Value.Date.Add(_time.Value);

            // Local time in user's timezone
            var offset =
                TimezoneFor.WibTimeZone.GetUtcOffset(localDateTime);

            var localOffset =
                new DateTimeOffset(localDateTime, offset);

            // Convert to UTC
            newValue = localOffset.ToUniversalTime();
        }

        if (newValue != Value)
        {
            Value = newValue;
            await ValueChanged.InvokeAsync(newValue);
        }
    }
}
