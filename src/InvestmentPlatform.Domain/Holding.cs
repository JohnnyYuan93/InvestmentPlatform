namespace InvestmentPlatform.Domain;

public sealed class Holding
{
    public Stock Stock { get; }
    public decimal Quantity { get; private set; }

    public Holding(Stock stock, decimal quantity)
    {
        //stock cannot be null, quantity must bigger than zero
        ArgumentNullException.ThrowIfNull(stock);
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        Stock = stock;
        Quantity = quantity;
    }
}
