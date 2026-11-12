using InvestmentPlatform.Domain.Services;

namespace InvestmentPlatform.Domain.Tests;

public class PortfolioServiceTests
{
    [Fact]
    public void CalculateValue_UsesInjectedPricesAndCurrentQuotes()
    {
        var apple = new Stock("AAPL", "Apple");
        var microsoft = new Stock("MSFT", "Microsoft");
        var prices = new FakePriceService();
        prices.SetPrice(apple, new Money(100m, "USD"));
        prices.SetPrice(microsoft, new Money(200m, "USD"));
        var service = new PortfolioService(prices);
        var holdings = new[] { new Holding(apple, 2.5m), new Holding(microsoft, 3m) };

        Assert.Equal(new Money(850m, "USD"), service.CalculateHoldingValue(holdings, "USD"));
        prices.SetPrice(apple, new Money(120m, "USD"));
        Assert.Equal(new Money(900m, "USD"), service.CalculateHoldingValue(holdings, "USD"));
    }

    [Fact]
    public void CalculateValue_EmptyHoldingsReturnZero() =>
        Assert.Equal(new Money(0m, "NZD"), new PortfolioService(new FakePriceService()).CalculateHoldingValue([], "NZD"));

    [Fact]
    public void CalculateValue_MissingPriceFailsExplicitly() =>
        Assert.Throws<KeyNotFoundException>(() => new PortfolioService(new FakePriceService()).CalculateHoldingValue([new Holding(new Stock("AAPL", "Apple"), 1m)], "USD"));

    [Fact]
    public void CalculateValue_MixedCurrenciesFailExplicitly()
    {
        var stock = new Stock("AAPL", "Apple");
        var prices = new FakePriceService();
        prices.SetPrice(stock, new Money(10m, "NZD"));
        Assert.Throws<InvalidOperationException>(() => new PortfolioService(prices).CalculateHoldingValue([new Holding(stock, 1m)], "USD"));
    }

    [Fact]
    public void Constructor_RejectsNullDependency() =>
        Assert.Throws<ArgumentNullException>(() => new PortfolioService(null!));

    [Fact]
    public void FakePriceService_RejectsNegativePrice() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new FakePriceService().SetPrice(new Stock("AAPL", "Apple"), new Money(-1m, "USD")));
}
