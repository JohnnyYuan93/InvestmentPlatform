using InvestmentPlatform.Domain.Exception;

namespace InvestmentPlatform.Domain;

public class Account
{
    public Guid Id { get; } = Guid.NewGuid();
    
    public string Name { get; private set; }
    
    public decimal CashBalance { get; private set; }
    
    //can be null
    public string? Description { get; set; }

    public Account(string name)
    {
        if  (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Account name cannot be empty",nameof(name));
        }
        Name = name.Trim();
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount));
        }
        CashBalance += amount;
    }

    public void Withdraw(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        if (amount > CashBalance)
        {
            throw new InsufficientFundsException($"{nameof(amount)} can't be greater than {nameof(CashBalance)}");
        }
        CashBalance -= amount;
    }
    
}
