using System.Threading.Tasks;

namespace ECommersAI.Services.Interfaces
{
    public interface IExchangeRateService
    {
        Task<decimal> GetRateToUsdAsync(string currency);
        Task<decimal> ConvertFromUsdAsync(decimal usdPrice, string currency);
    }
}
