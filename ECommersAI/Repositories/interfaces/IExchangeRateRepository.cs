using ECommersAI.Models.Entities;

namespace ECommersAI.Repositories.interfaces
{
    public interface IExchangeRateRepository : IRepository<ExchangeRate>
    {
        Task<ExchangeRate?> GetLatestByCurrencyAsync(string currency);
    }
}