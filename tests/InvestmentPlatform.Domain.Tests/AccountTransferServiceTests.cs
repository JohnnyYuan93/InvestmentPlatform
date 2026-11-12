using InvestmentPlatform.Domain.Exception;
using InvestmentPlatform.Domain.Services;

namespace InvestmentPlatform.Domain.Tests;

public class AccountTransferServiceTests
{
    private readonly InMemoryAccountRepository _repository = new();

    private Account CreateAccount(decimal balance)
    {
        var account = new Account("Learner");
        if (balance > 0m) account.Deposit(balance);
        _repository.Add(account);
        return account;
    }

    [Fact]
    public void Transfer_MissingSource_ThrowsWithoutChangingTarget()
    {
        var target = CreateAccount(20m);
        var service = new AccountTransferService(_repository);

        Assert.Throws<KeyNotFoundException>(() => service.Transfer(Guid.NewGuid(), target.Id, 10m));
        Assert.Equal(20m, target.CashBalance);
    }

    [Fact]
    public void Transfer_MissingTarget_ThrowsWithoutChangingSource()
    {
        var source = CreateAccount(100m);
        var service = new AccountTransferService(_repository);

        Assert.Throws<KeyNotFoundException>(() => service.Transfer(source.Id, Guid.NewGuid(), 10m));
        Assert.Equal(100m, source.CashBalance);
    }

    [Fact]
    public void Transfer_SameAccount_ThrowsWithoutChangingBalance()
    {
        var account = CreateAccount(100m);
        var service = new AccountTransferService(_repository);

        Assert.Throws<InvalidOperationException>(() => service.Transfer(account.Id, account.Id, 10m));
        Assert.Equal(100m, account.CashBalance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Transfer_NonPositiveAmount_ThrowsWithoutChangingBalances(int amount)
    {
        var source = CreateAccount(100m);
        var target = CreateAccount(20m);
        var service = new AccountTransferService(_repository);

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => service.Transfer(source.Id, target.Id, amount));
        Assert.Equal("amount", exception.ParamName);
        Assert.Equal(100m, source.CashBalance);
        Assert.Equal(20m, target.CashBalance);
    }

    [Fact]
    public void Transfer_InsufficientFunds_ThrowsWithoutChangingBalances()
    {
        var source = CreateAccount(10m);
        var target = CreateAccount(20m);
        var service = new AccountTransferService(_repository);

        Assert.Throws<InsufficientFundsException>(() => service.Transfer(source.Id, target.Id, 11m));
        Assert.Equal(10m, source.CashBalance);
        Assert.Equal(20m, target.CashBalance);
    }

    [Fact]
    public void Transfer_TargetOverflow_ThrowsBeforeDeductingSource()
    {
        var source = CreateAccount(10m);
        var target = CreateAccount(decimal.MaxValue);
        var service = new AccountTransferService(_repository);

        Assert.Throws<OverflowException>(() => service.Transfer(source.Id, target.Id, 1m));
        Assert.Equal(10m, source.CashBalance);
        Assert.Equal(decimal.MaxValue, target.CashBalance);
    }

    [Theory]
    [InlineData(25, 75, 45)]
    [InlineData(100, 0, 120)]
    public void Transfer_ValidAmount_UpdatesBothBalances(int amount, int expectedSource, int expectedTarget)
    {
        var source = CreateAccount(100m);
        var target = CreateAccount(20m);
        var service = new AccountTransferService(_repository);

        service.Transfer(source.Id, target.Id, amount);

        Assert.Equal((decimal)expectedSource, source.CashBalance);
        Assert.Equal((decimal)expectedTarget, target.CashBalance);
        Assert.Equal(120m, source.CashBalance + target.CashBalance);
    }

    [Fact]
    public void Constructor_NullRepository_Throws() =>
        Assert.Throws<ArgumentNullException>(() => new AccountTransferService(null!));
}
