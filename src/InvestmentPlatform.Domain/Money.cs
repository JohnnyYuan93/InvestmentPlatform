namespace InvestmentPlatform.Domain;

// A record supplies value equality; properties remain immutable after construction.
public sealed record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currency);
        if (currency.Trim().Length != 3)
            throw new ArgumentException("Currency must be a three-letter code.", nameof(currency));
        if (!currency.Trim().All(char.IsAsciiLetter))
            throw new ArgumentException("Currency must contain only letters.", nameof(currency));
        Amount = amount;
        Currency = currency.Trim().ToUpperInvariant();
    }

    public Money Add(Money other)
    {
        ArgumentNullException.ThrowIfNull(other);
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add different currencies.");
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Multiply(decimal quantity) => new(Amount * quantity, Currency);
}
