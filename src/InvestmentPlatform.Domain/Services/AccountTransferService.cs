using System;
using InvestmentPlatform.Domain.Exception;

namespace InvestmentPlatform.Domain.Services;

public class AccountTransferService(IAccountRepository accountRepository)
{
    private readonly IAccountRepository _accountRepository = accountRepository
        ?? throw new ArgumentNullException(nameof(accountRepository));

    public void Transfer(Guid fromAccountId, Guid toAccountId, decimal amount)
    {
        var fromAccount = _accountRepository.GetById(fromAccountId)
            ?? throw new KeyNotFoundException($"Source account {fromAccountId} was not found.");
        var toAccount = _accountRepository.GetById(toAccountId)
            ?? throw new KeyNotFoundException($"Target account {toAccountId} was not found.");

        if (fromAccount.Id == toAccount.Id)
        {
            throw new InvalidOperationException("Cannot transfer to the same account.");
        }

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
        
        if (fromAccount.CashBalance < amount)
        {
            throw new InsufficientFundsException("Source account has insufficient funds.");
        }

        // Validate the target's capacity before changing either balance.discard the result.
        _ = checked(toAccount.CashBalance + amount);

        fromAccount.Withdraw(amount);
        toAccount.Deposit(amount);
    }
}
