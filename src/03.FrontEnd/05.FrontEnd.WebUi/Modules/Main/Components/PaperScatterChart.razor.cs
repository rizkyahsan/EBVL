using ApexCharts;
using EBVL.FrontEnd.Logics.Modules.Examples.Orders.GetOrders;
using EBVL.Shared.Dto.Modules.Examples.Orders.GetOrders;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Components;

public partial class PaperScatterChart
{
    private IEnumerable<OrderItem> _orders = [];
    private IEnumerable<OrderItem> _trendOrders = [];
    private ApexChartOptions<OrderItem> _options = new();

    protected override async Task OnInitializedAsync()
    {
        _options = new()
        {
            Tooltip = new Tooltip { Shared = false, Intersect = true },
            Markers = new Markers { Size = 6 }
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

            var lowestOrder = _orders.OrderBy(x => x.DiscountPercentage).First();
            var highestOrder = _orders.OrderByDescending(x => x.DiscountPercentage).First();

            _trendOrders = [lowestOrder, highestOrder];
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

    private static string GetColor(OrderItem order)
    {
        if (order.GrossValue > 50000)
        {
            return "#3633FF";
        }

        return "#E51C15";
    }
}
