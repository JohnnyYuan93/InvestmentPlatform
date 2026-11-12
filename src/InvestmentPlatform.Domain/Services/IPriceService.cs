namespace InvestmentPlatform.Domain.Services;

public interface IPriceService
{
    Money GetPrice(Stock stock);
}
