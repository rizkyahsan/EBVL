using EBVL.FrontEnd.Logics.Modules.Examples.Dummies.GetDummies;
using EBVL.Shared.Dto.Modules.Examples.Dummies.GetDummies;

namespace EBVL.FrontEnd.WebUi.Modules.Examples.Features.Dummies.Pages;

public partial class Index
{
    private readonly IndexModel _model = new()
    {
        Angka1 = 1,
        Angka2 = 2,
        SuatuDateOnly = new DateOnly(1994, 10, 21),
        SuatuDateTime = new DateTime(1994, 10, 22, 16, 23, 42, DateTimeKind.Local),
        SuatuDateTimeOffset = new DateTimeOffset(1994, 10, 23, 16, 23, 43, TimeSpan.FromHours(7))
    };

    private IEnumerable<DummyItem> _items = [];

    protected override void OnInitialized()
    {
        _pageTitle = ExamplesDummiesDisplayTextFor.GetDummies;

        LoadBreadcrumbs();
    }

    protected override void LoadBreadcrumbs()
    {
        _breadcrumbItems =
        [
            MainBreadcrumbFor.Home,
            ExamplesBreadcrumbFor.Index,
            CommonBreadcrumbFor.Active(nameof(Dummies))
        ];
    }

    private async Task Submit()
    {
        ClearException();
        _isLoading = true;

        try
        {
            var query = new GetDummiesQuery
            {
                Angka1 = _model.Angka1,
                Angka2 = _model.Angka2,
                SuatuDateOnly = _model.SuatuDateOnly,
                SuatuDateTime = _model.SuatuDateTime,
                SuatuDateTimeOffset = _model.SuatuDateTimeOffset
            };

            var response = await Sender.Send(query);

            _items = response.Items;
        }
        catch (Exception exception)
        {
            _exception = exception;
        }
        finally
        {
            _isLoading = false;
        }
    }

    private sealed record IndexModel
    {
        public required int Angka1 { get; set; }
        public required int Angka2 { get; set; }
        public required DateOnly SuatuDateOnly { get; set; }
        public required DateTime SuatuDateTime { get; set; }
        public required DateTimeOffset SuatuDateTimeOffset { get; set; }
    }
}
