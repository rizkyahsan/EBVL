using ApexCharts;
using EBVL.FrontEnd.Logics.Modules.Examples.Orders.GetOrders;
using EBVL.Shared.Dto.Modules.Examples.Orders.GetOrders;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Components;

public partial class PaperDonutChart
{
    private IEnumerable<OrderItem> _orders = [];
    private ApexChartOptions<OrderItem> _options = new();

    protected override async Task OnInitializedAsync()
    {
        _options = new()
        {
            Chart = new Chart
            {
                Toolbar = new Toolbar
                {
                    Show = true,
                    Tools = new()
                    {
                        Download = true,
                        Selection = true,
                        Zoom = true,
                        Zoomin = true,
                        Zoomout = true,
                        Pan = true,
                        Reset = true
                    }
                }
            },
            PlotOptions = new PlotOptions
            {
                Pie = new PlotOptionsPie
                {
                    Donut = new PlotOptionsDonut
                    {
                        Labels = new DonutLabels
                        {
                            Total = new DonutLabelTotal
                            {
                                FontSize = "24px",
                                Color = "#D807B8",
                                Formatter = @"function (w) {return w.globals.seriesTotals.reduce((a, b) => { return (a + b) }, 0)}"
                            }
                        }
                    }
                }
            },
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
