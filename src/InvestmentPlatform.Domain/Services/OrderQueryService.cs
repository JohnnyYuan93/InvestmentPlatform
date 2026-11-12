using InvestmentPlatform.Domain.Enums;

namespace InvestmentPlatform.Domain.Services;

public class OrderQueryService
{
    public IEnumerable<Order> FilterOrders(
        IEnumerable<Order> orders,
        Func<Order, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(orders);
        ArgumentNullException.ThrowIfNull(predicate);

        return orders.Where(predicate);
    }

    public IEnumerable<Order> GetPendingOrders(
        IEnumerable<Order> orders)
    {
        ArgumentNullException.ThrowIfNull(orders);
        // TODO: Where
        return orders.Where(o => o.Status == OrderStatus.Pending);
    }

    public bool HasPendingOrders(
        IEnumerable<Order> orders)
    {
        ArgumentNullException.ThrowIfNull(orders);
        return orders.Any(o => o.Status == OrderStatus.Pending);
        // TODO: Any
    }

    public Order? FindOrderById(
        IEnumerable<Order> orders,
        Guid orderId)
    {
        ArgumentNullException.ThrowIfNull(orders);
        return orders.FirstOrDefault(o => o.Id == orderId);
    }

    public IEnumerable<Order> GetOrdersByQuantity(
        IEnumerable<Order> orders)
    {
        ArgumentNullException.ThrowIfNull(orders);
        // TODO: OrderByDescending
        return orders.OrderByDescending(o=> o.Quantity);
    }

    public Dictionary<string, decimal> GetTotalQuantityByStock(
        IEnumerable<Order> orders)
    {
        ArgumentNullException.ThrowIfNull(orders);
        // TODO: GroupBy + ToDictionary + Sum
        return orders.GroupBy(o=>o.Stock.Symbol).ToDictionary(
            k => k.Key,
            v => v.Sum(o=> o.Quantity));
    }
}
