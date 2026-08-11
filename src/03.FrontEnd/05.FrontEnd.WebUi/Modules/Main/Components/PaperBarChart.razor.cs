using EBVL.FrontEnd.Logics.Modules.Examples.Orders.GetOrders;
using EBVL.Shared.Dto.Modules.Examples.Orders.GetOrders;

namespace EBVL.FrontEnd.WebUi.Modules.Main.Components;

public partial class PaperBarChart
{
    private IEnumerable<OrderItem> _orders = [];

    protected override async Task OnInitializedAsync()
    {
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
