using InvestmentPlatform.Domain.Enums;

namespace InvestmentPlatform.Domain;

public sealed class Order
{
    public Guid Id { get; } = Guid.NewGuid();
    public Account Account { get; }
    public Stock Stock { get; }
    public OrderType Type { get; }
    public decimal Quantity { get; }
    public Money UnitPrice { get; }
    public Money Total => UnitPrice.Multiply(Quantity);
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    public Order(Account account, Stock stock, OrderType type, decimal quantity, Money unitPrice)
    {
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(stock);
        ArgumentNullException.ThrowIfNull(unitPrice);
        if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type));
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (unitPrice.Amount < 0) throw new ArgumentOutOfRangeException(nameof(unitPrice));
        Account = account;
        Stock = stock;
        Type = type;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public void Execute()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("Cannot execute order if the order is not pending.");
        }
        Status = OrderStatus.Complete;
    }
}
