namespace InvestmentPlatform.Domain.Services;

public class TransactionFeeService
{
    public decimal CalculateFee(decimal amount, Func<decimal, decimal> feeCalculator)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        }

        ArgumentNullException.ThrowIfNull(feeCalculator);

        var fee = feeCalculator(amount);
        if (fee < 0)
        {
            throw new InvalidOperationException("The calculated fee cannot be negative.");
        }

        return fee;
    }

    public void ProcessFee(
        decimal amount,
        Func<decimal, decimal> feeCalculator,
        Action<decimal> onFeeCalculated)
    {
        ArgumentNullException.ThrowIfNull(onFeeCalculated);

        var fee = CalculateFee(amount, feeCalculator);
        onFeeCalculated(fee);
    }
}
