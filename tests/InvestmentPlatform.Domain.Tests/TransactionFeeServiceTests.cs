using InvestmentPlatform.Domain.Services;

namespace InvestmentPlatform.Domain.Tests;

public class TransactionFeeServiceTests
{
    private readonly TransactionFeeService _service = new();

    [Fact]
    public void CalculateFee_StandardRate_ReturnsPercentageOfAmount()
    {
        var fee = _service.CalculateFee(1_000m, amount => amount * 0.01m);

        Assert.Equal(10m, fee);
    }

    [Fact]
    public void CalculateFee_FixedFee_ReturnsFixedAmount()
    {
        var fee = _service.CalculateFee(1_000m, _ => 5m);

        Assert.Equal(5m, fee);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CalculateFee_NonPositiveAmount_ThrowsArgumentOutOfRangeException(decimal amount)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            _service.CalculateFee(amount, value => value * 0.01m));

        Assert.Equal("amount", exception.ParamName);
    }

    [Fact]
    public void CalculateFee_NullCalculator_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            _service.CalculateFee(100m, null!));

        Assert.Equal("feeCalculator", exception.ParamName);
    }

    [Fact]
    public void CalculateFee_NegativeResult_ThrowsInvalidOperationException() =>
        Assert.Throws<InvalidOperationException>(() =>
            _service.CalculateFee(100m, _ => -1m));

    [Fact]
    public void ProcessFee_CalculatesFeeAndPassesItToCallback()
    {
        decimal? receivedFee = null;

        _service.ProcessFee(200m, amount => amount * 0.025m, fee => receivedFee = fee);

        Assert.Equal(5m, receivedFee);
    }

    [Fact]
    public void ProcessFee_NullCallback_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            _service.ProcessFee(100m, _ => 5m, null!));

        Assert.Equal("onFeeCalculated", exception.ParamName);
    }
}
