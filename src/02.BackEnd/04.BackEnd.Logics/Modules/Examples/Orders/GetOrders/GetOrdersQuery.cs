using EBVL.Shared.Dto.Modules.Examples.Orders.GetOrders;

namespace EBVL.BackEnd.Logics.Modules.Examples.Orders.GetOrders;

public sealed record GetOrdersQuery : IRequest<GetOrdersResponse>
{
}

public sealed class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, GetOrdersResponse>
{
    public async Task<GetOrdersResponse> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        var orders = new List<OrderItem>
        {
            new() { CustomerName = "Odio Corporation", Country = "Sweden", OrderDate = DateTimeOffset.Now.AddDays(-12), GrossValue = 34531, DiscountPercentage = 21 },
            new() { CustomerName = "Odio Corporation", Country = "Sweden", OrderDate = DateTimeOffset.Now.AddDays(-100), GrossValue = 2800, DiscountPercentage = 12 },
            new() { CustomerName = "Odio Corporation", Country = "Sweden", OrderDate = DateTimeOffset.Now.AddDays(-128), GrossValue = 12532, DiscountPercentage = 24 },
            new() { CustomerName = "Odio Corporation", Country = "Sweden", OrderDate = DateTimeOffset.Now.AddDays(-232), GrossValue = 1400, DiscountPercentage = 65 },
            new() { CustomerName = "Odio Corporation", Country = "Sweden", OrderDate = DateTimeOffset.Now.AddDays(-321), GrossValue = 22000, DiscountPercentage = 10 },
            new() { CustomerName = "Odio Corporation", Country = "Sweden", OrderDate = DateTimeOffset.Now.AddDays(-400), GrossValue = 3000, DiscountPercentage = 17 },
            new() { CustomerName = "Nascetur AB", Country = "Sweden", OrderDate = DateTimeOffset.Now.AddDays(-17), GrossValue = 2134, DiscountPercentage = 10 },
            new() { CustomerName = "Nascetur AB", Country = "Sweden", OrderDate = DateTimeOffset.Now.AddDays(-27), GrossValue = 11345, DiscountPercentage = 12 },
            new() { CustomerName = "Nascetur AB", Country = "Sweden", OrderDate = DateTimeOffset.Now.AddDays(-124), GrossValue = 63400, DiscountPercentage = 79 },
            new() { CustomerName = "Nascetur AB", Country = "Sweden", OrderDate = DateTimeOffset.Now.AddDays(-299), GrossValue = 1235, DiscountPercentage = 12 },
            new() { CustomerName = "Nascetur AB", Country = "Sweden", OrderDate = DateTimeOffset.Now.AddDays(-372), GrossValue = 44000, DiscountPercentage = 11 },
            new() { CustomerName = "Nascetur AB", Country = "Sweden", OrderDate = DateTimeOffset.Now.AddDays(-410), GrossValue = 17000, DiscountPercentage = 5 },
            new() { CustomerName = "Justo Eu Institute", Country = "Spain", OrderDate = DateTimeOffset.Now.AddDays(-10), GrossValue = 12000, DiscountPercentage = 23 },
            new() { CustomerName = "Justo Eu Institute", Country = "Spain", OrderDate = DateTimeOffset.Now.AddDays(-13), GrossValue = 92800, DiscountPercentage = 48 },
            new() { CustomerName = "Justo Eu Institute", Country = "Spain", OrderDate = DateTimeOffset.Now.AddDays(-45), GrossValue = 12532, DiscountPercentage = 24 },
            new() { CustomerName = "Justo Eu Institute", Country = "Spain", OrderDate = DateTimeOffset.Now.AddDays(-60), GrossValue = 1400, DiscountPercentage = 12 },
            new() { CustomerName = "Justo Eu Institute", Country = "Spain", OrderDate = DateTimeOffset.Now.AddDays(-150), GrossValue = 22000, DiscountPercentage = 10 },
            new() { CustomerName = "Justo Eu Institute", Country = "Spain", OrderDate = DateTimeOffset.Now.AddDays(-200), GrossValue = 3000, DiscountPercentage = 17 },
            new() { CustomerName = "Ani Vent", Country = "France", OrderDate = DateTimeOffset.Now.AddDays(-17), GrossValue = 2134, DiscountPercentage = 34 },
            new() { CustomerName = "Ani Vent", Country = "France", OrderDate = DateTimeOffset.Now.AddDays(-27), GrossValue = 11345, DiscountPercentage = 12 },
            new() { CustomerName = "Ani Vent", Country = "France", OrderDate = DateTimeOffset.Now.AddDays(-124), GrossValue = 17002, DiscountPercentage = 32 },
            new() { CustomerName = "Cali Inc", Country = "France", OrderDate = DateTimeOffset.Now.AddDays(-10), GrossValue = 77000, DiscountPercentage = 17 },
            new() { CustomerName = "Cali Inc", Country = "France", OrderDate = DateTimeOffset.Now.AddDays(-110), GrossValue = 120000, DiscountPercentage = 23 },
            new() { CustomerName = "Cali Inc", Country = "France", OrderDate = DateTimeOffset.Now.AddDays(-243), GrossValue = 44000, DiscountPercentage = 8 },
            new() { CustomerName = "Chart Inc", Country = "Brazil", OrderDate = DateTimeOffset.Now.AddDays(-11), GrossValue = 2345, DiscountPercentage = 11 },
            new() { CustomerName = "Chart Inc", Country = "Brazil", OrderDate = DateTimeOffset.Now.AddDays(-14), GrossValue = 34567, DiscountPercentage = 22 },
            new() { CustomerName = "Chart Inc", Country = "Brazil", OrderDate = DateTimeOffset.Now.AddDays(-121), GrossValue = 45662, DiscountPercentage = 23 },
            new() { CustomerName = "Chart Inc", Country = "Brazil", OrderDate = DateTimeOffset.Now.AddDays(-11), GrossValue = 66000, DiscountPercentage = 11 },
            new() { CustomerName = "Chart Inc", Country = "Brazil", OrderDate = DateTimeOffset.Now.AddDays(-90), GrossValue = 10000, DiscountPercentage = 8 },
            new() { CustomerName = "Chart Inc", Country = "Brazil", OrderDate = DateTimeOffset.Now.AddDays(-123), GrossValue = 69000, DiscountPercentage = 25 }
        };

        return new GetOrdersResponse
        {
            Items = orders
        };
    }
}
