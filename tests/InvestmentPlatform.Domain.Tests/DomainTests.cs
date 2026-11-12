using InvestmentPlatform.Domain;
using InvestmentPlatform.Domain.Enums;

namespace InvestmentPlatform.Domain.Tests;

public class DomainTests
{
    [Fact]
    public void Account_PreservesExistingProperties()
    {
        var account = new Account("Learner");
        account.Deposit(100m);
        Assert.Equal("Learner", account.Name);
        Assert.Equal(100m, account.CashBalance);
    }

    [Fact]
    public void Money_HasValueEqualityAndNormalizesCurrency()
    {
        Assert.Equal(new Money(10m, "USD"), new Money(10m, " usd "));
        var original = new Money(10m, "USD");
        Assert.Equal(new Money(15m, "USD"), original.Add(new Money(5m, "USD")));
        Assert.Equal(10m, original.Amount);
    }

    [Theory]
    [InlineData("")]
    [InlineData("US")]
    [InlineData("123")]
    public void Money_RejectsInvalidCurrency(string currency) =>
        Assert.ThrowsAny<ArgumentException>(() => new Money(1m, currency));

    [Fact]
    public void Money_RejectsMixedCurrencies() =>
        Assert.Throws<InvalidOperationException>(() => new Money(1m, "USD").Add(new Money(1m, "NZD")));

    [Fact]
    public void Stock_NormalizesSymbol()
    {
        var stock = new Stock(" aapl ", " Apple ");
        Assert.Equal("AAPL", stock.Symbol);
        Assert.Equal("Apple", stock.Name);
        Assert.Throws<ArgumentException>(() => new Stock(" ", "Apple"));
    }

    [Fact]
    public void Order_CalculatesTotal()
    {
        var order = new Order(new Account("Test"), new Stock("AAPL", "Apple"), OrderType.Buy, 2.5m, new Money(100m, "USD"));
        Assert.Equal(new Money(250m, "USD"), order.Total);
        Assert.NotEqual(Guid.Empty, order.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Models_RejectNonPositiveQuantities(int quantity)
    {
        var stock = new Stock("AAPL", "Apple");
        Assert.Throws<ArgumentOutOfRangeException>(() => new Holding(stock, quantity));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Order(new Account("Test"), stock, OrderType.Buy, quantity, new Money(10m, "USD")));
    }

    [Fact]
    public void Order_RejectsInvalidSideAndNegativePrice()
    {
        var stock = new Stock("AAPL", "Apple");
        Assert.Throws<ArgumentOutOfRangeException>(() => new Order(new Account("Test"), stock, (OrderType)99, 1m, new Money(10m, "USD")));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Order(new Account("Test"), stock, OrderType.Buy, 1m, new Money(-1m, "USD")));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Account_RejectsInvalidDepositAmount(decimal amount)
    {
        var account = new Account("Test");
        Assert.Throws<ArgumentOutOfRangeException>(
            () => account.Deposit(amount));
    }
    
}
