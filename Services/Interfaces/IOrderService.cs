
using ECommersAI.DTOs.Order;

namespace ECommersAI.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetAllAsync();
        Task<OrderDto?> GetByIdAsync(Guid id);
        Task<OrderDto> CreateAsync(CreateOrderRequest request);
        Task<bool> DeleteAsync(Guid id);
        Task<OrderDto> AutoGenerateAsync(AutoGenerateOrderRequest request);
    }
}
