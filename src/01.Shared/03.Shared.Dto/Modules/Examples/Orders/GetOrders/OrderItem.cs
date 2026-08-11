namespace EBVL.Shared.Dto.Modules.Examples.Orders.GetOrders;

public sealed record OrderItem
{
    public Guid OrderId { get; init; } = Guid.CreateVersion7();
    public required string CustomerName { get; init; }
    public required string Country { get; init; }
    public required DateTimeOffset OrderDate { get; init; }
    public required decimal GrossValue { get; init; }
    public required decimal DiscountPercentage { get; init; }
    public decimal NetValue => GrossValue * (1 - (DiscountPercentage / 100));
}
