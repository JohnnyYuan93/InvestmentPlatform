namespace InvestmentPlatform.Domain.Services;

public sealed class PortfolioService
{
    private readonly IPriceService _priceService;

    public PortfolioService(IPriceService priceService)
    {
        ArgumentNullException.ThrowIfNull(priceService);
        _priceService = priceService;
    }

    // Market value of holdings only; excludes account cash and currency conversion.
    public Money CalculateHoldingValue(IEnumerable<Holding> holdings, string currency)
    {
        ArgumentNullException.ThrowIfNull(holdings);
        var total = new Money(0m, currency);
        foreach (var holding in holdings)
        {
            ArgumentNullException.ThrowIfNull(holding);
            var price = _priceService.GetPrice(holding.Stock);
            if (price.Amount < 0) throw new InvalidOperationException("Price cannot be negative.");
            total = total.Add(price.Multiply(holding.Quantity));
        }
        return total;
    }

    public Money CalculatePortfolioValue(Account account, IEnumerable<Holding> holdings, string currency)
    {
        var holdingTotal = CalculateHoldingValue(holdings, currency);
        var accountTotal = new Money(account.CashBalance, currency);
        return accountTotal.Add(holdingTotal);
    }
}
