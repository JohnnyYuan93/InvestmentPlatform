namespace InvestmentPlatform.Domain;

public sealed class Stock
{
    public string Symbol { get; }
    public string Name { get; }

    public Stock(string symbol, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Symbol = symbol.Trim().ToUpperInvariant();
        Name = name.Trim();
    }
}
