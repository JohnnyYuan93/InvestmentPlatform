namespace InvestmentPlatform.Domain.Services;

public class AccountService(IAccountRepository accountRepository)
{
    public Account? FindAccount(Guid id)
    {
        return accountRepository.GetById(id);
    }
}