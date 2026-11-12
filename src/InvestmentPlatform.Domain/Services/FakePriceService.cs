namespace InvestmentPlatform.Domain.Services;

// In-memory implementation for learning and tests; no network requests.
public sealed class FakePriceService : IPriceService
{
    private readonly Dictionary<string, Money> _prices = new(StringComparer.OrdinalIgnoreCase);

    public void SetPrice(Stock stock, Money price)
    {
        ArgumentNullException.ThrowIfNull(stock);
        ArgumentNullException.ThrowIfNull(price);
        if (price.Amount < 0) throw new ArgumentOutOfRangeException(nameof(price));
        _prices[stock.Symbol] = price;
    }

    public Money GetPrice(Stock stock)
    {
        ArgumentNullException.ThrowIfNull(stock);
        return _prices.TryGetValue(stock.Symbol, out var price)
            ? price
            : throw new KeyNotFoundException($"No price configured for {stock.Symbol}.");
    }
}
