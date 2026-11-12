using InvestmentPlatform.Domain.Enums;
using InvestmentPlatform.Domain.Services;

namespace InvestmentPlatform.Domain.Tests;

public class OrderQueryServiceTests
{
    private readonly OrderQueryService _service = new();

    [Fact]
    public void FilterOrders_CustomPredicate_ReturnsMatchingOrders()
    {
        var smallBuy = CreateOrder("AAPL", 1m);
        var largeBuy = CreateOrder("MSFT", 5m);
        var largeSell = CreateOrder("AAPL", 10m, OrderType.Sell);

        var result = _service
            .FilterOrders([smallBuy, largeBuy, largeSell], order =>
                order.Type == OrderType.Buy && order.Quantity >= 5m)
            .ToArray();

        Assert.Equal([largeBuy], result);
    }

    [Fact]
    public void FilterOrders_DoesNotEvaluatePredicateUntilEnumerated()
    {
        var predicateCalls = 0;

        var result = _service.FilterOrders([CreateOrder()], order =>
        {
            predicateCalls++;
            return true;
        });

        Assert.Equal(0, predicateCalls);
        Assert.Single(result);
        Assert.Equal(1, predicateCalls);
    }

    [Fact]
    public void FilterOrders_NullArguments_ThrowArgumentNullException()
    {
        var ordersException = Assert.Throws<ArgumentNullException>(() =>
            _service.FilterOrders(null!, _ => true));
        var predicateException = Assert.Throws<ArgumentNullException>(() =>
            _service.FilterOrders([CreateOrder()], null!));

        Assert.Equal("orders", ordersException.ParamName);
        Assert.Equal("predicate", predicateException.ParamName);
    }

    [Fact]
    public void GetPendingOrders_MixedStatuses_ReturnsOnlyPendingOrders()
    {
        // Arrange: change status through the public domain method.
        var firstPending = CreateOrder("AAPL", 2m);
        var completed = CreateOrder("MSFT", 3m);
        completed.Execute();
        var secondPending = CreateOrder("MSFT", 4m);
        Order[] orders = [firstPending, completed, secondPending];

        // Act
        var result = _service.GetPendingOrders(orders).ToArray();

        // Assert
        Assert.Equal(new[] { firstPending, secondPending }, result);
        Assert.Equal(OrderStatus.Complete, completed.Status);
        Assert.Equal(3, orders.Length);
    }

    [Fact]
    public void GetPendingOrders_AllCompleted_ReturnsEmpty()
    {
        var order = CreateOrder();
        order.Execute();

        Assert.Empty(_service.GetPendingOrders([order]));
    }

    [Fact]
    public void HasPendingOrders_PendingAfterCompleted_ReturnsTrue()
    {
        var completed = CreateOrder();
        completed.Execute();

        Assert.True(_service.HasPendingOrders([completed, CreateOrder()]));
    }

    [Fact]
    public void HasPendingOrders_AllCompleted_ReturnsFalse()
    {
        var completed = CreateOrder();
        completed.Execute();

        Assert.False(_service.HasPendingOrders([completed]));
    }

    [Fact]
    public void FindOrderById_ExistingId_ReturnsMatchingInstance()
    {
        var first = CreateOrder();
        var target = CreateOrder();

        Assert.Same(target, _service.FindOrderById([first, target], target.Id));
    }

    [Fact]
    public void FindOrderById_UnknownId_ReturnsNull() =>
        Assert.Null(_service.FindOrderById([CreateOrder()], Guid.NewGuid()));

    [Fact]
    public void GetOrdersByQuantity_ReturnsDescendingOrderWithoutChangingInput()
    {
        var small = CreateOrder(quantity: 1.5m);
        var large = CreateOrder(quantity: 10m);
        var middle = CreateOrder(quantity: 3m);
        Order[] orders = [small, large, middle];

        var result = _service.GetOrdersByQuantity(orders).ToArray();

        Assert.Equal(new[] { large, middle, small }, result);
        Assert.Equal(new[] { small, large, middle }, orders);
    }

    [Fact]
    public void GetOrdersByQuantity_EqualQuantities_KeepsBothOrders()
    {
        var first = CreateOrder(quantity: 2m);
        var second = CreateOrder(quantity: 2m);

        Assert.Equal(new[] { first, second }, _service.GetOrdersByQuantity([first, second]).ToArray());
    }

    [Fact]
    public void GetTotalQuantityByStock_GroupsBySymbolAndSumsFractionalQuantities()
    {
        // Separate Stock instances with the same normalized symbol must share a group.
        Order[] orders =
        [
            CreateOrder("aapl", 1.5m),
            CreateOrder("MSFT", 4m),
            CreateOrder("AAPL", 2.25m)
        ];

        var result = _service.GetTotalQuantityByStock(orders);

        Assert.Equal(2, result.Count);
        Assert.Equal(3.75m, result["AAPL"]);
        Assert.Equal(4m, result["MSFT"]);
    }

    [Fact]
    public void GetTotalQuantityByStock_IncludesCompletedAndSellOrders()
    {
        // This query totals order quantities, not net holdings or pending orders only.
        var completedBuy = CreateOrder("AAPL", 2m);
        completedBuy.Execute();
        var sell = CreateOrder("AAPL", 3m, OrderType.Sell);

        var result = _service.GetTotalQuantityByStock([completedBuy, sell]);

        Assert.Single(result);
        Assert.Equal(5m, result["AAPL"]);
    }

    [Fact]
    public void Queries_EmptyInput_ReturnEmptyFalseOrNull()
    {
        Order[] orders = [];

        Assert.Empty(_service.GetPendingOrders(orders));
        Assert.False(_service.HasPendingOrders(orders));
        Assert.Null(_service.FindOrderById(orders, Guid.NewGuid()));
        Assert.Empty(_service.GetOrdersByQuantity(orders));
        Assert.Empty(_service.GetTotalQuantityByStock(orders));
    }

    [Theory]
    [InlineData(nameof(OrderQueryService.GetPendingOrders))]
    [InlineData(nameof(OrderQueryService.HasPendingOrders))]
    [InlineData(nameof(OrderQueryService.FindOrderById))]
    [InlineData(nameof(OrderQueryService.GetOrdersByQuantity))]
    [InlineData(nameof(OrderQueryService.GetTotalQuantityByStock))]
    public void Queries_NullInput_ThrowArgumentNullException(string method)
    {
        Action query = method switch
        {
            nameof(OrderQueryService.GetPendingOrders) => () => _service.GetPendingOrders(null!).ToArray(),
            nameof(OrderQueryService.HasPendingOrders) => () => _service.HasPendingOrders(null!),
            nameof(OrderQueryService.FindOrderById) => () => _service.FindOrderById(null!, Guid.NewGuid()),
            nameof(OrderQueryService.GetOrdersByQuantity) => () => _service.GetOrdersByQuantity(null!).ToArray(),
            nameof(OrderQueryService.GetTotalQuantityByStock) => () => _service.GetTotalQuantityByStock(null!),
            _ => throw new ArgumentOutOfRangeException(nameof(method))
        };

        var exception = Assert.Throws<ArgumentNullException>(query);
        Assert.Equal("orders", exception.ParamName);
    }

    private static Order CreateOrder(string symbol = "AAPL", decimal quantity = 1m,
        OrderType type = OrderType.Buy) =>
        new(new Account("Learner"), new Stock(symbol, symbol), type, quantity, new Money(10m, "USD"));
}
