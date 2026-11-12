namespace InvestmentPlatform.Domain.Services;

public class InMemoryAccountRepository : IAccountRepository
{
    private readonly Dictionary<Guid, Account> _accounts = new();

    public void Add(Account account)
    {
        if (account == null)
        {
            throw new ArgumentNullException(nameof(account));
        }
        _accounts.Add(account.Id, account);
    }

    public Account? GetById(Guid id)
    {
        return _accounts.TryGetValue(id, out Account? account) ? account : null;
    }

    public IReadOnlyCollection<Account> GetAll()
    {
        return _accounts.Values;
    }
}