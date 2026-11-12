namespace InvestmentPlatform.Domain.Services;

public interface IAccountRepository
{ 
    void Add(Account account);
    Account? GetById(Guid id);
    IReadOnlyCollection<Account> GetAll();
}