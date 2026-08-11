using ApexCharts;
using EBVL.FrontEnd.Logics.Modules.Examples.Orders.GetOrders;
using EBVL.Shared.Dto.Modules.Examples.Orders.GetOrders;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Components;

public partial class PaperLineChart
{
    private IEnumerable<OrderItem> _orders = [];
    private ApexChartOptions<OrderItem> _options = new();

    protected override async Task OnInitializedAsync()
    {
        _options = new()
        {
            Chart = new Chart
            {
                DropShadow = new DropShadow
                {
                    Enabled = true,
                    Color = "",
                    Top = 18,
                    Left = 7,
                    Blur = 10,
                    Opacity = 0.2d
                },
                Toolbar = new Toolbar
                {
                    Show = true
                }
            },
            DataLabels = new DataLabels
            {
                OffsetY = -6d
            },
            Grid = new Grid
            {
                BorderColor = "#e7e7e7",
                Row = new GridRow
                {
                    Colors = ["#f3f3f3", "transparent"],
                    Opacity = 0.5d
                }
            },
            Colors = ["#77B6EA", "#545454"],
            Markers = new Markers { Shape = MarkerShape.Circle, Size = 5, FillOpacity = new Opacity(0.8d) },
            Stroke = new Stroke { Curve = Curve.Smooth },
            Legend = new Legend
            {
                Position = LegendPosition.Bottom,
                HorizontalAlign = ApexCharts.Align.Right
            }
        };

        await GetData();
    }

    private async Task GetData()
    {
        try
        {
            _isLoading = true;

            ClearException();

            var query = new GetOrdersQuery();
            var response = await Sender.Send(query);

            await Task.Delay(new Random().Next(500, 3000));

            _orders = response.Items;
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
}
